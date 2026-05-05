using ApsMartChat.Data;
using ApsMartChat.DTOs;
using ApsMartChat.DTOs.FileTransfer;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApsMartChat.Services.File;

public class FileService : IFileService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public FileService(AppDbContext db, IWebHostEnvironment env, IMapper mapper)
    {
        _db = db;
        _env = env;
        _mapper = mapper;
    }

    private static readonly HashSet<string> TiposArquivosPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    private static readonly HashSet<string> ExtensoesArquivosPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".docx", ".xlsx"
    };

    // os primeiros bytes do arquivo que identificam o tipo real do arquivo => magic bytes
    private static readonly Dictionary<string, byte[]> MagicBytes = new()
    {
        { ".pdf",  new byte[]{0x25, 0x50, 0x44, 0x46}},
        { ".docx", new byte[]{0x50, 0x4B, 0x03, 0x04}},
        { ".xlsx", new byte[]{0x50, 0x4B, 0x03, 0x04}},
    };

    private const long TamanhoMaxArqBytes = 200 * 1024 * 1024; // 200 MB

    private static async Task<bool> ValidarBytesIniciaisAsync(IFormFile file, string ext)
    {
        if (!MagicBytes.TryGetValue(ext, out var assinatura))
            throw new InvalidTypeFileException();

        // lê só os primeiros bytes, sem carregar o arquivo inteiro
        var buffer = new byte[assinatura.Length];
        await using var stream = file.OpenReadStream();
        var lidos = await stream.ReadAsync(buffer, 0, buffer.Length);

        if (lidos < assinatura.Length)
            throw new InvalidTypeFileException();

        return buffer.SequenceEqual(assinatura);
    }

    public async Task<List<FileTransferResponseDTO>> UploadDeArquivoAsync(List<IFormFile> files, string username, int roomId, string baseUrl)
    {
        if (files == null || !files.Any())
            throw new InvalidTypeFileException("Nenhum arquivo enviado.");

        if (files.Count > 5)
            throw new MaxFilesExceededException("Máximo de 5 arquivos.");

        // valida existência da room
        var roomExists = await _db.ChatRooms.AnyAsync(r => r.Id == roomId);
        if (!roomExists)
            throw new NotFoundException($"A sala com ID {roomId} não existe.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username)
            ?? throw new NotFoundException($"O usuário {username} não foi localizado.");

        var erros = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > TamanhoMaxArqBytes)
            {
                erros.Add($"{file.FileName}: excede 200MB");
                continue;
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                erros.Add("Arquivo sem nome detectado");
                continue;
            }

            var ext = Path.GetExtension(file.FileName);

            if (!ExtensoesArquivosPermitidas.Contains(ext))
            {
                erros.Add($"{file.FileName}: tipo não permitido");
                continue;
            }

            if (!await ValidarBytesIniciaisAsync(file, ext))
            {
                erros.Add($"{file.FileName}: conteúdo inválido");
                continue;
            }
        }

        if (erros.Any())
            throw new InvalidTypeFileException(string.Join(" | ", erros));

        var uploadsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);

        var transfers = new List<FileTransfer>();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName);
            var storedName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsPath, storedName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var transfer = new FileTransfer
            {
                NomeOriginal = file.FileName,
                NomeGeradoCript = storedName,
                TipoConteudo = file.ContentType,
                TamanhoBytes = file.Length,
                UploaderId = user.Id,
                RoomId = roomId
            };

            transfers.Add(transfer);
        }

        _db.FileTransfers.AddRange(transfers);
        await _db.SaveChangesAsync();

        return _mapper.Map<List<FileTransferResponseDTO>>(transfers);
    }

    public async Task<(Stream stream, string contentType, string fileName)> DownloadDeArquivoAsync(int fileId)
    {
        var transfer = await _db.FileTransfers.FindAsync(fileId);

        if (transfer is null)
            throw new NotFoundException();

        var path = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", transfer.NomeGeradoCript);

        if (!System.IO.File.Exists(path))
            throw new NotFoundException("O arquivo não existe no caminho informado.");

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read); // FileMode.Open pode dar erro e precisa ser tratado => pesquisar formas
        return (stream, transfer.TipoConteudo, transfer.NomeOriginal);
    }

    public async Task<List<FileTransferResponseDTO>> GetFilesByRoomAsync(int roomId)
    {
        var idRoomExist = await _db.ChatRooms.AnyAsync(c => c.Id == roomId);

        if (!idRoomExist)
            throw new NotFoundException($"Chat Room {roomId} não localizada.");

        return await _db.FileTransfers
            .Include(f => f.Uploader)
            .Where(f => f.RoomId == roomId)
            .OrderByDescending(f => f.UploadedAt)
            .ProjectTo<FileTransferResponseDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}

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

    public async Task<FileTransferResponseDTO> UploadDeArquivoAsync(IFormFile file, string username, int roomId, string baseUrl)
    {
        if (file.Length > TamanhoMaxArqBytes)
            throw new FileLargerThan200MbException("Arquivo excede 200 MB.");

        if (string.IsNullOrWhiteSpace(file.FileName))
            throw new InvalidTypeFileException();

        var ext = Path.GetExtension(file.FileName);
        if (!ExtensoesArquivosPermitidas.Contains(ext))
            throw new InvalidTypeFileException("Tipo de arquivo não permitido. Use .pdf, .docx ou .xlsx.");

        if (!await ValidarBytesIniciaisAsync(file, ext))
            throw new InvalidTypeFileException("O conteúdo do arquivo não corresponde à extensão informada.");

        // valida existencia da room
        var roomExists = await _db.ChatRooms.AnyAsync(r => r.Id == roomId);
        if (!roomExists)
            throw new NotFoundException($"A sala com ID {roomId} não existe.");

        // Salva no disco
        var uploadsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);

        var storedName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(uploadsPath, storedName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Salva no banco 
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username)
            ?? throw new NotFoundException($"O usuário {username} não foi localizado.");

        var transfer = new FileTransfer
        {
            NomeOriginal = file.FileName,
            NomeGeradoCript = storedName,
            TipoConteudo = file.ContentType,
            TamanhoBytes = file.Length,
            UploaderId = user.Id,
            RoomId = roomId
        };

        _db.FileTransfers.Add(transfer);
        await _db.SaveChangesAsync();

        return _mapper.Map<FileTransferResponseDTO>(transfer);
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

using ApsMartChat.Data;
using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.DTOs.FileTransfer;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using ApsMartChat.Models.Enum;
using ApsMartChat.Services.File;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;

namespace ApsMartChat.Tests.Services.FileTests;

public class FileTransferServiceTest
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly Mock<IWebHostEnvironment> _env = new();
    private readonly FileService _service;

    public FileTransferServiceTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);

        _mapper = new MapperConfiguration(expression =>
        {
            expression.AddProfile<Profiles.FileTransferProfile>();
            expression.AddProfile<Profiles.ChatRoomProfile>();
        }).CreateMapper();

        _service = new FileService(_db, _env.Object, _mapper);

        _env.Setup(e => e.WebRootPath).Returns("wwwroot");
    }

    // Testes de Upload de Arquivo

    [Fact]
    public async Task Sucesso_Ao_Fazer_Upload_De_Arquivo()
    {
        // file
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // adicionando usuario no banco
        _db.Users.Add(new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        });

        _db.ChatRooms.Add(new Models.ChatRoom { Name = "sala teste" });

        await _db.SaveChangesAsync();

        // act
        var result = await _service.UploadDeArquivoAsync(FilesList, "leoxsamuel1", 1, "");

        Assert.NotNull(result);
        for (int i = 0; i < FilesList.Count(); i++)
        {
            Assert.Equal("teste.pdf", result[i].NomeOriginal);
        }
    }

    [Fact]
    public async Task Erro_Ao_Fazer_Upload_De_Arquivo_Conteudo_Nao_Bate_Com_Extensao()
    {
        // file
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.xlsx";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // adicionando usuario no banco
        _db.Users.Add(new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        });

        _db.ChatRooms.Add(new Models.ChatRoom { Name = "sala teste" });

        await _db.SaveChangesAsync();

        // act
        await Assert.ThrowsAsync<InvalidTypeFileException>(
                    () => (_service.UploadDeArquivoAsync(FilesList, "leoxsamuel1", 1, "")));
    }

    [Fact]
    public async Task Erro_Ao_Fazer_Upload_De_Arquivo_Tamanho_Maior_Do_Que_200MB()
    {
        var mockfile = new Mock<IFormFile>();

        mockfile.Setup(f => f.Length).Returns(300_000_000);
        mockfile.Setup(f => f.FileName).Returns("arquivo_maior_do_que_200mb.pdf");

        IFormFile file = mockfile.Object;

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // criar chat room e usuario
        _db.Users.Add(new User
        {
            Username = "LeonardoXSamuel",
            PasswordHash = "hash123456",
            DisplayName = "Leonardo Samuel",
            Role = UserRole.Coordenador
        });

        var chatroom = new Models.ChatRoom
        {
            Name = "sala teste"
        };

        _db.ChatRooms.Add(chatroom);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<FileLargerThan200MbException>(
            () => (_service.UploadDeArquivoAsync(FilesList, "LeonardoXSamuel", 1, "")));
    }

    [Fact]
    public async Task Erro_Ao_Fazer_Upload_De_Arquivo_Tipo_Não_Permitido()
    {
        var mockfile = new Mock<IFormFile>();

        mockfile.Setup(f => f.Length).Returns(300);
        mockfile.Setup(f => f.Headers.ContentType).Returns("git");
        mockfile.Setup(f => f.ContentType).Returns("git");
        mockfile.Setup(f => f.FileName).Returns("arquivo_tipo_nao_permitido.git");

        IFormFile file = mockfile.Object;

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // criar chat room e usuario
        _db.Users.Add(new User
        {
            Username = "LeonardoXSamuel",
            PasswordHash = "hash123456",
            DisplayName = "Leonardo Samuel",
            Role = UserRole.Coordenador
        });

        var chatroom = new Models.ChatRoom
        {
            Name = "sala teste"
        };

        _db.ChatRooms.Add(chatroom);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidTypeFileException>(
            () => (_service.UploadDeArquivoAsync(FilesList, "LeonardoXSamuel", 1, "")));
    }

    [Fact]
    public async Task Erro_Ao_Fazer_Upload_De_Arquivo_ChatRoom_Inexistente()
    {
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // criar chat room e usuario
        _db.Users.Add(new User
        {
            Username = "LeonardoXSamuel",
            PasswordHash = "hash123456",
            DisplayName = "Leonardo Samuel",
            Role = UserRole.Coordenador
        });

        var chatroom = new Models.ChatRoom
        {
            Name = "sala teste"
        };

        _db.ChatRooms.Add(chatroom);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(
           () => (_service.UploadDeArquivoAsync(FilesList, "LeonardoXSamuel", 101, "")));
    }

    [Fact]
    public async Task Erro_Ao_Fazer_Upload_De_Arquivo_Username_Inexistente()
    {
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // criar chat room e usuario
        _db.Users.Add(new User
        {
            Username = "LeonardoXSamuel",
            PasswordHash = "hash123456",
            DisplayName = "Leonardo Samuel",
            Role = UserRole.Coordenador
        });

        var chatroom = new Models.ChatRoom
        {
            Name = "sala teste"
        };

        _db.ChatRooms.Add(chatroom);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(
           () => (_service.UploadDeArquivoAsync(FilesList, "sarahXrabettiC", 1, "")));
    }

    // Testes de Download de Arquivo
    [Fact]
    public async Task Sucesso_Ao_Fazer_Download_De_Arquivo()
    {
        //realizando o upload

        // file
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // adicionando usuario no banco
        _db.Users.Add(new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        });

        _db.ChatRooms.Add(new Models.ChatRoom { Name = "tste sala" });
        await _db.SaveChangesAsync();

        var result = await _service.UploadDeArquivoAsync(FilesList, "leoxsamuel1", 1, "");

        // fazer dowload
        FileTransfer fileInBD = await _db.FileTransfers.FirstOrDefaultAsync(f => f.Id == 1);
        var response = await _service.DownloadDeArquivoAsync(fileInBD.Id);

        Assert.NotNull(response);
        Assert.Equal("application/pdf", response.contentType);
        Assert.Equal("teste.pdf", response.fileName);

        response.stream.Position = 0;

        using var reader = new StreamReader(response.stream);
        var contentDownloaded = await reader.ReadToEndAsync();

        Assert.Equal("%PDF-1.4 fake content", contentDownloaded);
        Assert.True(response.stream.Length > 0);
    }

    [Fact]
    public async Task Erro_Ao_Fazer_Download_De_Arquivo_FileId_Nao_Encontrado()
    {
        //realizando o upload

        // file
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);

        // adicionando usuario no banco
        _db.Users.Add(new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        });

        _db.ChatRooms.Add(new Models.ChatRoom { Name = "tsste sala" });
        await _db.SaveChangesAsync();
        await _service.UploadDeArquivoAsync(FilesList, "leoxsamuel1", 1, "");

        // act => fazer dowload
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DownloadDeArquivoAsync(54));
    }

    [Fact]
    public async Task Erro_Ao_Fazer_Download_De_Arquivo_Nao_Existe_No_Path_Indicado()
    {
        _env.Setup(e => e.WebRootPath).Returns("wwwroot");

        _db.FileTransfers.Add(new FileTransfer
        {
            NomeGeradoCript = "arquivo_inexistente.pdf",
            NomeOriginal = "teste.pdf",
            TipoConteudo = "application/pdf",
            TamanhoBytes = 100,
            UploaderId = 1,
            RoomId = 1
        });

        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DownloadDeArquivoAsync(1)
        );
    }

    // Testes de retornar arquivos por sala
    [Fact]
    public async Task Sucesso_Ao_Retornar_Arquivos_da_ChatRoom()
    {
        Models.ChatRoom ChatRoom = new Models.ChatRoom { Name = "sala teste" };
        _db.Add(ChatRoom);
        await _db.SaveChangesAsync();

        // file
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        // file 2
        var content2 = "%PDF-1.4 fake content";
        var fileName2 = "teste.pdf";
        var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(content2));

        IFormFile file2 = new FormFile(stream2, 0, stream2.Length, "file", fileName2)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);
        FilesList.Add(file2);

        // adicionando usuario no banco
        _db.Users.Add(new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        });
        await _db.SaveChangesAsync();

        await _service.UploadDeArquivoAsync(FilesList, "leoxsamuel1", ChatRoom.Id, "");
        await _db.SaveChangesAsync();

        List<FileTransferResponseDTO> ListFilesInRoom = await _service
            .GetFilesByRoomAsync(ChatRoom.Id);

        Assert.NotEmpty(ListFilesInRoom);
    }

    [Fact]
    public async Task Erro_Ao_Retornar_Arquivos_da_ChatRoom_IdChatRoom_Inexistente()
    {
        Models.ChatRoom ChatRoom = new Models.ChatRoom { Name = "sala teste" };
        _db.Add(ChatRoom);
        await _db.SaveChangesAsync();

        // file
        var content = "%PDF-1.4 fake content";
        var fileName = "teste.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        // file 2
        var content2 = "%PDF-1.4 fake content";
        var fileName2 = "teste.pdf";
        var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(content2));

        IFormFile file2 = new FormFile(stream2, 0, stream2.Length, "file", fileName2)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };


        List<IFormFile> FilesList = new List<IFormFile>();
        FilesList.Add(file);
        FilesList.Add(file2);

        // adicionando usuario no banco
        _db.Users.Add(new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        });
        await _db.SaveChangesAsync();

        await _service.UploadDeArquivoAsync(FilesList, "leoxsamuel1", ChatRoom.Id, "");
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetFilesByRoomAsync(14)
        );
    }

}
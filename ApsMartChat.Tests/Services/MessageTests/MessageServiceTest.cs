using ApsMartChat.Data;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using ApsMartChat.Models.Enum;
using ApsMartChat.Services.Message;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApsMartChat.Tests.Services.MessageTests;

public class MessageServiceTest
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly MessageService _service;

    public MessageServiceTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);

        _mapper = new MapperConfiguration(expression =>
        {
            expression.AddProfile<Profiles.MessageProfile>();
            expression.AddProfile<Profiles.UserProfile>();
            expression.AddProfile<Profiles.FileTransferProfile>();
            expression.AddProfile<Profiles.ChatRoomProfile>();
        }).CreateMapper();

        _service = new MessageService(_db, _mapper);
    }

    [Fact]
    public async Task Sucesso_Ao_Salvar_Mensagem()
    {
        // adicionando usuario no banco
        var user = new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // adicionando ChatRoom no banco
        var cr = new Models.ChatRoom
        {
            Name = "sala teste"
        };
        _db.ChatRooms.Add(cr);
        await _db.SaveChangesAsync();

        string content = "conteudo da mensagem";
        string username = user.Username;
        int chatRoomId = cr.Id;

        var ResponseDTO = await _service.SaveMessageAsync(content, username, chatRoomId);

        Assert.NotNull(ResponseDTO);
        Assert.Equal(ResponseDTO.Content, content);
    }

    [Fact]
    public async Task Erro_Ao_Salvar_Mensagem_UserName_Nao_Localizado()
    {
        // adicionando ChatRoom no banco
        var cr = new Models.ChatRoom
        {
            Name = "sala teste"
        };
        _db.ChatRooms.Add(cr);
        await _db.SaveChangesAsync();

        string content = "conteudo da mensagem";
        string username = "Harry_Potter147";
        int chatRoomId = cr.Id;

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.SaveMessageAsync(content, username, chatRoomId));
    }

    [Fact]
    public async Task Erro_Ao_Salvar_Mensagem_ChatRoom_Nao_Localizada()
    {
        // adicionando usuario no banco
        var user = new User
        {
            Username = "leoxsamuel1",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        string content = "conteudo da mensagem";
        string username = "leoxsamuel1";
        int chatRoomId = 204;

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.SaveMessageAsync(content, username, chatRoomId));
    }

    [Fact]
    public async Task Sucesso_Ao_Retornar_Historico_de_Mensagens()
    {
        // adicionando usuario no banco
        var user = new User
        {
            Username = "Harry_Potter147",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // adicionando ChatRoom no banco
        var cr = new Models.ChatRoom
        {
            Name = "sala teste"
        };
        _db.ChatRooms.Add(cr);
        await _db.SaveChangesAsync();

        string content = "conteudo da mensagem";
        string content2 = "conteudo da segunda mensagem";
        string username = "Harry_Potter147";
        int chatRoomId = cr.Id;

        await _service.SaveMessageAsync(content, username, cr.Id);
        await _service.SaveMessageAsync(content2, username, cr.Id);

        var ListMsgs = await _service.GetHistoryOfMessagesAsync(cr.Id);

        Assert.NotEmpty(ListMsgs);
        Assert.Collection(ListMsgs,
            m => Assert.Equal(content2, m.Content),
            m => Assert.Equal(content, m.Content)
        );

        Assert.True(ListMsgs[1].SentAt >= ListMsgs[0].SentAt);
    }

    [Fact]
    public async Task Erro_Ao_Retornar_Historico_de_Mensagens_Lista_de_Mensagens_Vazia()
    {
        // adicionando usuario no banco
        var user = new User
        {
            Username = "Harry_Potter147",
            PasswordHash = "hash123456",
            DisplayName = "Leo Leonardo",
            Role = UserRole.Coordenador
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // adicionando ChatRoom no banco
        var cr = new Models.ChatRoom
        {
            Name = "sala teste"
        };
        _db.ChatRooms.Add(cr);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetHistoryOfMessagesAsync(cr.Id));
    }

}


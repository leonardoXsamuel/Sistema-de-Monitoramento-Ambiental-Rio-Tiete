using ApsMartChat.Data;
using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ApsMartChat.Tests.Services.ChatRoom;

public class ChatRoomServiceTest
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly Mock<IWebHostEnvironment> _env = new();
    private readonly ChatRoomService _service;

    public ChatRoomServiceTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);

        _mapper = new MapperConfiguration(expression =>
        {
            expression.AddProfile<Profiles.ChatRoomProfile>();
        }).CreateMapper();

        _service = new ChatRoomService(_db, _env.Object, _mapper);
    }


    [Fact]
    public async Task Deve_Criar_ChatRoom_Com_Sucesso()
    {
        // Arrange
        var dto = new ChatRoomCreateDTO("sala de teste");

        // Act
        var result = await _service.CriarChatRoomAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("sala de teste", result.Name);

        var roomNoBanco = await _db.ChatRooms.FirstOrDefaultAsync(x => x.Name == result.Name);
        Assert.NotNull(roomNoBanco);
        Assert.Equal("sala de teste", roomNoBanco.Name);
    }

    [Fact]
    public async Task Deve_Dar_Erro_Ao_Criar_ChatRoom_NomeSala_Vazio()
    {
        var dto = new ChatRoomCreateDTO(" ");

        await Assert.ThrowsAsync<InvalidInputException>(
            () => _service.CriarChatRoomAsync(dto));
    }

    [Fact]
    public async Task Deve_Dar_Sucesso_Ao_Criar_ChatRoom_NomeSala_1Caracter()
    {
        var dto = new ChatRoomCreateDTO("x");
        var ChatRoom = _service.CriarChatRoomAsync(dto);

        var CR = await _db.ChatRooms.FirstOrDefaultAsync(x => x.Name == dto.Name);

        Assert.NotNull(ChatRoom);
        Assert.Equal("x", CR.Name);
    }

    [Fact]
    public async Task Deve_Falhar_Se_ChatRoom_Não_For_Localizada_Para_UpdateAsync()
    {
        ChatRoomUpdateDTO dto = new ChatRoomUpdateDTO("sala teste");

        int roomId = 4747;

        await Assert.ThrowsAsync<NotFoundException>(async () => await _service.AlterarNomeChatRoom(roomId, dto));
    }


    [Fact]
    public async Task Atualizar_Nome_Da_ChatRoom_Com_Exito()
    {
        // Arrange
        var createDto = new ChatRoomCreateDTO("sala teste");
        var created = await _service.CriarChatRoomAsync(createDto);

        var chatRoom = await _db.ChatRooms.FirstOrDefaultAsync(cr => cr.Name == "sala teste");
        Assert.NotNull(chatRoom); // garante que o create funcionou antes de tentar o update

        var updateDto = new ChatRoomUpdateDTO("novo nome de sala");

        // Act
        var result = await _service.AlterarNomeChatRoom(chatRoom.Id, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(created.Name, result.Name);
        Assert.Equal(updateDto.Name, result.Name);
    }

    [Fact]
    public async Task Retornar_ChatRooms_Com_Exito()
    {
        var chatRooms = new List<Models.ChatRoom>{
            new Models.ChatRoom { Name = "sala teste" },
            new Models.ChatRoom { Name = "sala teste2" }
        };

        await _db.ChatRooms.AddRangeAsync(chatRooms);
        await _db.SaveChangesAsync();

        // Act
        var result = (await _service.RetornarTodasAsChatRooms()).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(chatRooms[0].Name, result[0].Name);
        Assert.Equal(chatRooms[1].Name, result[1].Name);
    }

    [Fact]
    public async Task Nao_Deve_Retornar_ChatRooms_Banco_Vazio()
    {
        await Assert.ThrowsAsync<NotFoundException>(
           () => _service.RetornarTodasAsChatRooms());
    }
}
using ApsMartChat.Data;
using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using ApsMartChat.Services.ChatRoom;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class ChatRoomService : IChatRoomService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public ChatRoomService(AppDbContext db, IWebHostEnvironment env, IMapper mapper)
    {
        _db = db;
        _env = env;
        _mapper = mapper;
    }

    public async Task<ChatRoomResponseDTO> CriarChatRoomAsync(ChatRoomCreateDTO dto)
    {

        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length < 1)
        {
            throw new InvalidInputException("o NOME DA SALA inserido é inválido.");
        }

        var room = new ChatRoom
        {
            Name = dto.Name
        };

        _db.ChatRooms.Add(room);
        await _db.SaveChangesAsync();

        return _mapper.Map<ChatRoomResponseDTO>(room);
    }

    public async Task<ChatRoomResponseDTO> AlterarNomeChatRoom(int roomId, ChatRoomUpdateDTO chatRoomUpdateDTO)
    {
        var chatRoomExist = await _db.ChatRooms.FindAsync(roomId) ?? throw new NotFoundException();

        if (string.IsNullOrWhiteSpace(chatRoomUpdateDTO.Name))
            throw new InvalidInputException("Nome inválido");

        chatRoomExist.Name = chatRoomUpdateDTO.Name;
        await _db.SaveChangesAsync();

        return _mapper.Map<ChatRoomResponseDTO>(chatRoomExist);
    }

    public async Task<IEnumerable<ChatRoomResponseDTO>> RetornarTodasAsChatRooms(int page, int pageSize)
    {
        var ListCRs = await _db.ChatRooms
        .OrderBy(c => c.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        if (!ListCRs.Any())
                throw new NotFoundException("Não existem ChatRooms registradas.");

        return ListCRs.Select(cr => _mapper.Map<ChatRoomResponseDTO>(cr));
    }
}

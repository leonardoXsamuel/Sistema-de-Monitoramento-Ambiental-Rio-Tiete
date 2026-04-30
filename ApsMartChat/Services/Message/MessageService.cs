using ApsMartChat.Data;
using ApsMartChat.DTOs;
using ApsMartChat.DTOs.Message;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApsMartChat.Services.Message;

public class MessageService : IMessageService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public MessageService(AppDbContext db, IMapper mapper) => (_db, _mapper) = (db, mapper);

    public async Task<MessageResponseDTO> SaveMessageAsync(string content, string username, int roomId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new NotFoundException();

        var message = new Models.Message
        {
            Content = content,
            SenderId = user.Id,
            RoomId = roomId,
            SentAt = DateTime.UtcNow
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        return _mapper.Map<MessageResponseDTO>(message);
    }

    public async Task<List<MessageResponseDTO>> GetHistoryOfMessagesAsync(int roomId, int page = 1, int pageSize = 50)
    {
        var ListMessages = await _db.Messages
            .Include(m => m.Sender)
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<MessageResponseDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (!ListMessages.Any())
            throw new NotFoundException("Não há histórico de mensagens registrado.");

        return ListMessages;
    }

    
}

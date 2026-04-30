using ApsMartChat.DTOs.Message;
using ApsMartChat.Models;

namespace ApsMartChat.Services.Message;

public interface IMessageService
{
    Task<MessageResponseDTO> SaveMessageAsync(string content, string username, int roomId);
    Task<List<MessageResponseDTO>> GetHistoryOfMessagesAsync(int roomId, int page = 1, int pageSize = 50);
    // adicionar funções de atualizar e deletar messages
}
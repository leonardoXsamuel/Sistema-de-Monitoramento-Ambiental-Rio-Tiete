using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.DTOs.User;

namespace ApsMartChat.DTOs.Message;

public class MessageResponseDTO
{
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public UserResponseDTO Sender { get; set; }
    public ChatRoomResponseDTO Room { get; set; }
}
using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.DTOs.User;

namespace ApsMartChat.DTOs.Message;

public record MessageResponseDTO (
    string Content,
    DateTime SentAt,
    UserResponseDTO Sender,
    ChatRoomResponseDTO Room
);

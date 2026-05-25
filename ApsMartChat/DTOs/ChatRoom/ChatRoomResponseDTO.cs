using ApsMartChat.DTOs.FileTransfer;
using ApsMartChat.DTOs.Message;

namespace ApsMartChat.DTOs.ChatRoom;

public record ChatRoomResponseDTO(
    int Id,
    string Name,
    DateTime CreatedAt,

    ICollection<MessageResponseDTO> Messages,
    ICollection<FileTransferResponseDTO> FileTransfers
)
{ }

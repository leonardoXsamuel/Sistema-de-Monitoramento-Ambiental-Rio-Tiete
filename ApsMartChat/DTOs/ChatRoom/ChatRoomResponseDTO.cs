using ApsMartChat.DTOs.FileTransfer;
using ApsMartChat.DTOs.Message;

namespace ApsMartChat.DTOs.ChatRoom;

public record ChatRoomResponseDTO(
    string Name,
    DateTimeOffset CreatedAt,

    ICollection<MessageResponseDTO> Messages,
    ICollection<FileTransferResponseDTO> FileTransfers
)
{ }

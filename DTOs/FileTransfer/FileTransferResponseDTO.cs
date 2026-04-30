using ApsMartChat.DTOs.ChatRoom;
using ApsMartChat.DTOs.User;

namespace ApsMartChat.DTOs.FileTransfer;

public record FileTransferResponseDTO(
    string NomeOriginal,
    string TipoConteudo,
    long TamanhoBytes,
    DateTime UploadedAt,
    string DownloadUrl,
    int? UploaderId,
    int? RoomId
);
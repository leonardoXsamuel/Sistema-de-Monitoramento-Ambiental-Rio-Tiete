using ApsMartChat.DTOs;
using ApsMartChat.DTOs.FileTransfer;

namespace ApsMartChat.Services.File;

public interface IFileService
{
    Task<FileTransferResponseDTO> UploadDeArquivoAsync(IFormFile file, string username, int roomId, string baseUrl);
    Task<(Stream stream, string contentType, string fileName)> DownloadDeArquivoAsync(int fileId);
    Task<List<FileTransferResponseDTO>> GetFilesByRoomAsync(int roomId);
}

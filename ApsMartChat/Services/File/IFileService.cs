using ApsMartChat.DTOs;
using ApsMartChat.DTOs.FileTransfer;

namespace ApsMartChat.Services.File;

public interface IFileService
{
    Task<List<FileTransferResponseDTO>> UploadDeArquivoAsync(List<IFormFile> files, string username, int roomId, string baseUrl);
    Task<(Stream stream, string contentType, string fileName)> DownloadDeArquivoAsync(int fileId);
    Task<List<FileTransferResponseDTO>> GetFilesByRoomAsync(int roomId);
}

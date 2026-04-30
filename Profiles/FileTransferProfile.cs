namespace ApsMartChat.Profiles;

using ApsMartChat.DTOs.FileTransfer;
using ApsMartChat.Models;
using AutoMapper;

public class FileTransferProfile : Profile
{
    public FileTransferProfile()
    {
        CreateMap<FileTransferCreateDTO, FileTransfer>();
        CreateMap<FileTransferUpdateDTO, FileTransfer>();
        CreateMap<FileTransfer, FileTransferResponseDTO>()
            .ConstructUsing(src => new FileTransferResponseDTO(
                src.NomeOriginal,
                src.TipoConteudo,
                src.TamanhoBytes,
                src.UploadedAt,
                $"/api/files/{src.Id}/download",
                src.UploaderId,
                src.RoomId
            ));
    }

}
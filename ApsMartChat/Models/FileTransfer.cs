using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.Models;

public class FileTransfer
{
    public int Id { get; set; }
    [StringLength(55, MinimumLength = 1)]
    public string NomeOriginal { get; set; } = string.Empty;  // nome original
    public string NomeGeradoCript { get; set; } = string.Empty;  // nome q vai ser criado no FileService 
    public string TipoConteudo { get; set; } = string.Empty;  // ex: application/pdf
    
    [Required]
    public long TamanhoBytes { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public int UploaderId { get; set; }
    [Required]
    public User Uploader { get; set; } = null!;

    public int RoomId { get; set; }
    [Required]
    public ChatRoom Room { get; set; } = null!;
}
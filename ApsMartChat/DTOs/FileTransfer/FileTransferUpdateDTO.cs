using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.FileTransfer;

public record FileTransferUpdateDTO([Required] [StringLength(55, MinimumLength = 1)] string NomeOriginal);
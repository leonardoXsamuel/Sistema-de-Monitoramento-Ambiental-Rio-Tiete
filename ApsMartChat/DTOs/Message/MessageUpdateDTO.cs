using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.Message;

public record MessageUpdateDTO
(
    [Required]
    [StringLength(400, MinimumLength = 1)]
    string Content
);

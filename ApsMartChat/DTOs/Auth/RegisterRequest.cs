using ApsMartChat.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.Auth;

public record RegisterRequest(

    [Required]
    [StringLength(55, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9._]+$", ErrorMessage = "Username só pode ter letras, números, ponto e underscore.")]
    string Username,

    [Required]
    [StringLength(100, MinimumLength = 6)]
    string Password,

    [Required]
    [StringLength(55, MinimumLength = 6)]
    string DisplayName,

    [Required]
    [StringLength(12, MinimumLength = 8, ErrorMessage = "UserRole deve ser Inspetor ou Coordenador")]
    string Role
);
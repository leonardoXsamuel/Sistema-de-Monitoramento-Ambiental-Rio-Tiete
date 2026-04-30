using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.Auth;

public record LoginRequest(
    
    [Required]
    [StringLength(55, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9._]+$", ErrorMessage = "Username só pode ter letras, números, ponto e underscore.")]
    string Username,

    [Required]
    string Password
    
);

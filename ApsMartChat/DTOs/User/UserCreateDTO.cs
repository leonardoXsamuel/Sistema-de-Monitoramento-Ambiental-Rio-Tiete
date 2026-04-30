using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.User;

public record UserCreateDTO
(
    [Required, StringLength(100, MinimumLength = 3)]
    string Username,

    [Required, StringLength(100, MinimumLength = 3)]
    string DisplayName
);

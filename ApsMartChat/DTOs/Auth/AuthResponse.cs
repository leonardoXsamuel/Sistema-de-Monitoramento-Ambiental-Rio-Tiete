using ApsMartChat.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace ApsMartChat.DTOs.Auth;

public record AuthResponse(
    string Token,
    string Username,
    string DisplayName,
    UserRole Role
);


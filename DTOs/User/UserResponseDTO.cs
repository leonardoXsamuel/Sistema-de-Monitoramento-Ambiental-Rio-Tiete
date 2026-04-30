namespace ApsMartChat.DTOs.User;

public record UserResponseDTO(
    string Username,
    string DisplayName,
    string Role,
    DateTime CreatedAt
);

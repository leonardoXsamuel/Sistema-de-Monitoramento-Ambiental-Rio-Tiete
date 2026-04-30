using ApsMartChat.DTOs.Auth;

namespace ApsMartChat.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse?> RegistrarUsuarioAsync(RegisterRequest request);
    Task<AuthResponse?> LoginDeUsuarioAsync(LoginRequest request);
}

using ApsMartChat.Data;
using ApsMartChat.DTOs.Auth;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using ApsMartChat.Models.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApsMartChat.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponse?> RegistrarUsuarioAsync(RegisterRequest req)
    {
        // Verifica se username já existe
        if (await _db.Users.AnyAsync(u => u.Username == req.Username))
            throw new UserExistsException($"O usuário {req.Username} já existe!");

        var role = Enum.TryParse<UserRole>(req.Role, true, out var parsed) ? parsed : UserRole.Inspetor;

        var user = new User
        {
            Username = req.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password), // linha em que a senha é criptografada
            DisplayName = req.DisplayName,
            Role = role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponse(
            GenerateToken(user),
            user.Username,
            user.DisplayName,
            user.Role
        );
    }

    public async Task<AuthResponse?> LoginDeUsuarioAsync(LoginRequest req)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == req.Username) ?? throw new NotFoundException($"O usuário {req.Username} não foi localizado!");

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new WrongPasswordException("A senha digitada está incorreta.");

        return new AuthResponse(
            GenerateToken(user),
            user.Username,
            user.DisplayName,
            user.Role
        );
    }

    private string GenerateToken(User user)
    {
        var secretKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),      
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("displayName", user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(120),
            signingCredentials: creds
        );


        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

}

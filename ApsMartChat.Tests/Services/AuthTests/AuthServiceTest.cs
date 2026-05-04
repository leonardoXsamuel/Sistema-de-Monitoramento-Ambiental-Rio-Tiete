using ApsMartChat.Data;
using ApsMartChat.DTOs.Auth;
using ApsMartChat.Exceptions;
using ApsMartChat.Models;
using ApsMartChat.Models.Enum;
using ApsMartChat.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ApsMartChat.Tests.Services.AuthTests;

public class AuthServiceTest
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly AuthService _service;

    public AuthServiceTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        _db = new AppDbContext(options);

        var inMemorySettings = new Dictionary<string, string>
        {
             {"Jwt:Key", "SUPER_SECRET_KEY_123456789_123456789_123456789"}
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _service = new AuthService(_db, _config);
    }

    // quebra DRY -> verificar melhor opcao nesse caso
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

    [Fact]
    public async Task Sucesso_Ao_Registrar_Usuario()
    {
        RegisterRequest req = new RegisterRequest("leonardoXsamuel", "Leomar2015@", "Leonardo Samuel", "Inspetor");

        var resp = await _service.RegistrarUsuarioAsync(req);

        Assert.NotNull(resp);
        Assert.Equal("leonardoXsamuel", resp.Username);
     
        var userDb = await _db.Users.FirstAsync();
        Assert.True(BCrypt.Net.BCrypt.Verify("Leomar2015@", userDb.PasswordHash));
    }

    [Fact]
    public async Task Erro_Ao_Registrar_Usuario_Credenciais_Já_Existentes()
    {
        var user = new User
        {
            Username = "leosam123XYZ",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Srbt34#$"), // linha em que a senha é criptografada
            DisplayName = "Leonardo Samuel",
            Role = UserRole.Inspetor
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        var req = new RegisterRequest("leosam123XYZ", "123@", "Leo", "Inspetor");

        await Assert.ThrowsAsync<UserExistsException>(
            () => _service.RegistrarUsuarioAsync(req));
    }

    [Fact]
    public async Task Sucesso_Ao_Logar_Usuario()
    {
        var user = new User
        {
            Username = "leosam123XYZ",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Srbt34#$"), // linha em que a senha é criptografada
            DisplayName = "Leonardo Samuel",
            Role = UserRole.Inspetor
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        var req = new LoginRequest("leosam123XYZ", "Srbt34#$");
        var response = await _service.LoginDeUsuarioAsync(req);

        Assert.Equal(req.Username, response.Username);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task Erro_Ao_Logar_Usuario_Username_Não_Localizado()
    {
        var req = new LoginRequest("LEONARDO_SAMUEL_XA4", "Leomar2020#$");

        await Assert.ThrowsAsync<NotFoundException>(
                    () => _service.LoginDeUsuarioAsync(req));
    }

    [Fact]
    public async Task Erro_Ao_Logar_Usuario_Password_Incorreto()
    {
        var user = new User
        {
            Username = "leosam123XYZ",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Srbt34#$"), // linha em que a senha é criptografada
            DisplayName = "Leonardo Samuel",
            Role = UserRole.Inspetor
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        var LogReq = new LoginRequest("leosam123XYZ", "LEOMAR201512#$");

        await Assert.ThrowsAsync<WrongPasswordException>(
                    () => _service.LoginDeUsuarioAsync(LogReq));
    }

    [Fact]
    public async Task Sucesso_Ao_Gerar_Token()
    {
        var req = new RegisterRequest("leo123", "123@", "Leo", "Inspetor");

        var resp = await _service.RegistrarUsuarioAsync(req);

        Assert.NotNull(resp.Token);
        Assert.NotEmpty(resp.Token);
    }

    [Fact]
    public async Task Deve_Lancar_Erro_Quando_JwtKey_Nao_Config()
    {
        var configVazio = new ConfigurationBuilder().Build();
        var service = new AuthService(_db, configVazio);

        var req = new RegisterRequest("leo123", "123@", "Leo", "Inspetor");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegistrarUsuarioAsync(req));
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppGastos.Api.Common;
using AppGastos.Api.Data;
using AppGastos.Api.Dtos;
using AppGastos.Api.Logica;           
using AppGastos.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AppGastos.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtOptions _jwtOptions;

    public AuthService(AppDbContext db, IOptions<JwtOptions> jwtOptions)
    {
        _db = db;
        _jwtOptions = jwtOptions.Value;
    }

    // Registrar un usuario nuevo (ACÁ está el cambio)
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Antes había una línea que normalizaba el email y tres "if".
        // Ahora la regla vive en RegistroValidator, que devuelve el email ya normalizado.
        var email = RegistroValidator.Validar(request.Email, request.Password, request.Name);

        var exists = await _db.Users.AnyAsync(u => u.Email == email);
        if (exists)
            throw new ConflictException("Ya existe un usuario con ese email.");

        var user = new User
        {
            Email = email,
            Name = request.Name.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponse(GenerateToken(user), user.Email, user.Name);
    }

    // Iniciar sesión (NO cambia)
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Email o contraseña incorrectos.");

        return new AuthResponse(GenerateToken(user), user.Email, user.Name);
    }

    // Generar el token JWT (NO cambia)
    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
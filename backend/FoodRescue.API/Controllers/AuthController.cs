using FoodRescue.API.Data;
using FoodRescue.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodRescue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FoodRescueDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(
        FoodRescueDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // REGISTRAR USUARIO
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new
            {
                message = "El correo ya está registrado"
            });
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Usuario registrado correctamente"
        });
    }

    // LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new
            {
                message = "Correo o contraseña incorrectos"
            });
        }

        var token = GenerateToken(user);

        return Ok(new
        {
            message = "Inicio de sesión correcto",
            token,
            user = new
            {
                user.Id,
                user.Name,
                user.Email
            }
        });
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(
    string Name,
    string Email,
    string Password
);

public record LoginRequest(
    string Email,
    string Password
);
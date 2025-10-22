using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarSystem.Infrastructure;
using BarSystem.Domain.Entities;
using BarSystem.Application.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace BarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly BarContext _context;

    public UsuariosController(BarContext context)
    {
        _context = context;
    }

    // POST api/usuarios/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UsuarioRegisterDto dto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("El usuario ya existe.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok("Usuario registrado con éxito.");
    }

    // POST api/usuarios/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(UsuarioLoginDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (usuario == null) return Unauthorized("Usuario no encontrado.");

        if (usuario.PasswordHash != HashPassword(dto.Password))
            return Unauthorized("Contraseña incorrecta.");

        return Ok($"Bienvenido {usuario.Nombre}!");
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}

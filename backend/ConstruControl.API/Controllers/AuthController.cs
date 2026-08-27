using ConstruControl.Application.DTOs.Auth;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;

    public AuthController(IUsuarioRepository usuarioRepository, IJwtService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(request.Email);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
        {
            return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });
        }

        var token = _jwtService.GenerarToken(usuario);

        return Ok(new LoginResponseDto
        {
            Token = token,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString()
        });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")] // Solo un Admin ya logueado puede crear usuarios
    public async Task<ActionResult> Register([FromBody] RegisterRequestDto request)
    {
        return await CrearUsuario(request);
    }

    /// <summary>
    /// Crea el primer usuario Admin del sistema. Solo funciona si NO existe
    /// ningún usuario todavía — evita que se use como puerta trasera después.
    /// </summary>
    [HttpPost("seed-admin")]
    [AllowAnonymous]
    public async Task<ActionResult> SeedAdmin([FromBody] RegisterRequestDto request)
    {
        if (await _usuarioRepository.ExisteAlgunUsuarioAsync())
        {
            return Conflict(new { mensaje = "Ya existen usuarios en el sistema. Use /register con un Admin autenticado." });
        }

        request.Rol = "Admin";
        return await CrearUsuario(request);
    }

    private async Task<ActionResult> CrearUsuario(RegisterRequestDto request)
    {
        if (await _usuarioRepository.ExisteEmailAsync(request.Email))
        {
            return Conflict(new { mensaje = "Ya existe un usuario con ese email." });
        }

        if (!Enum.TryParse<RolUsuario>(request.Rol, out var rol))
        {
            return BadRequest(new { mensaje = "Rol inválido. Use: Admin, JefeObra, Bodeguero, Empleado." });
        }

        var usuario = new Usuario
        {
            NombreCompleto = request.NombreCompleto,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rol = rol
        };

        await _usuarioRepository.AgregarAsync(usuario);
        await _usuarioRepository.GuardarCambiosAsync();

        return Created(string.Empty, new { mensaje = "Usuario creado correctamente." });
    }
}

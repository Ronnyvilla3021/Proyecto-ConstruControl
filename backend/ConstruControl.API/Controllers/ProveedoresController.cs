using ConstruControl.Application.DTOs.Proveedores;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorRepository _proveedorRepository;

    public ProveedoresController(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProveedorResponseDto>>> ObtenerTodos()
    {
        var proveedores = await _proveedorRepository.ObtenerTodosAsync();
        return Ok(proveedores.Select(MapearADto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProveedorResponseDto>> ObtenerPorId(int id)
    {
        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(id);
        if (proveedor is null)
        {
            return NotFound(new { mensaje = $"No existe un proveedor con id {id}." });
        }
        return Ok(MapearADto(proveedor));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Bodeguero")]
    public async Task<ActionResult<ProveedorResponseDto>> Crear([FromBody] ProveedorRequestDto request)
    {
        var proveedor = new Proveedor
        {
            Nombre = request.Nombre,
            Contacto = request.Contacto,
            Telefono = request.Telefono,
            Email = request.Email
        };

        await _proveedorRepository.AgregarAsync(proveedor);
        await _proveedorRepository.GuardarCambiosAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = proveedor.Id }, MapearADto(proveedor));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Bodeguero")]
    public async Task<ActionResult<ProveedorResponseDto>> Actualizar(int id, [FromBody] ProveedorRequestDto request)
    {
        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(id);
        if (proveedor is null)
        {
            return NotFound(new { mensaje = $"No existe un proveedor con id {id}." });
        }

        proveedor.Nombre = request.Nombre;
        proveedor.Contacto = request.Contacto;
        proveedor.Telefono = request.Telefono;
        proveedor.Email = request.Email;

        _proveedorRepository.Actualizar(proveedor);
        await _proveedorRepository.GuardarCambiosAsync();

        return Ok(MapearADto(proveedor));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(id);
        if (proveedor is null)
        {
            return NotFound(new { mensaje = $"No existe un proveedor con id {id}." });
        }

        _proveedorRepository.EliminarLogico(proveedor);
        await _proveedorRepository.GuardarCambiosAsync();

        return NoContent();
    }

    private static ProveedorResponseDto MapearADto(Proveedor proveedor) => new()
    {
        Id = proveedor.Id,
        Nombre = proveedor.Nombre,
        Contacto = proveedor.Contacto,
        Telefono = proveedor.Telefono,
        Email = proveedor.Email
    };
}

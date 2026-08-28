using ConstruControl.Application.DTOs.Empleados;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public EmpleadosController(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmpleadoResponseDto>>> ObtenerTodos()
    {
        var empleados = await _empleadoRepository.ObtenerTodosAsync();
        return Ok(empleados.Select(MapearADto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmpleadoResponseDto>> ObtenerPorId(int id)
    {
        var empleado = await _empleadoRepository.ObtenerPorIdAsync(id);
        if (empleado is null)
        {
            return NotFound(new { mensaje = $"No existe un empleado con id {id}." });
        }
        return Ok(MapearADto(empleado));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,JefeObra")]
    public async Task<ActionResult<EmpleadoResponseDto>> Crear([FromBody] EmpleadoRequestDto request)
    {
        var empleado = new Empleado
        {
            Nombre = request.Nombre,
            Cargo = request.Cargo,
            FechaIngreso = request.FechaIngreso
        };

        await _empleadoRepository.AgregarAsync(empleado);
        await _empleadoRepository.GuardarCambiosAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = empleado.Id }, MapearADto(empleado));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,JefeObra")]
    public async Task<ActionResult<EmpleadoResponseDto>> Actualizar(int id, [FromBody] EmpleadoRequestDto request)
    {
        var empleado = await _empleadoRepository.ObtenerPorIdAsync(id);
        if (empleado is null)
        {
            return NotFound(new { mensaje = $"No existe un empleado con id {id}." });
        }

        empleado.Nombre = request.Nombre;
        empleado.Cargo = request.Cargo;
        empleado.FechaIngreso = request.FechaIngreso;

        _empleadoRepository.Actualizar(empleado);
        await _empleadoRepository.GuardarCambiosAsync();

        return Ok(MapearADto(empleado));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var empleado = await _empleadoRepository.ObtenerPorIdAsync(id);
        if (empleado is null)
        {
            return NotFound(new { mensaje = $"No existe un empleado con id {id}." });
        }

        _empleadoRepository.EliminarLogico(empleado);
        await _empleadoRepository.GuardarCambiosAsync();

        return NoContent();
    }

    private static EmpleadoResponseDto MapearADto(Empleado empleado) => new()
    {
        Id = empleado.Id,
        Nombre = empleado.Nombre,
        Cargo = empleado.Cargo,
        FechaIngreso = empleado.FechaIngreso
    };
}

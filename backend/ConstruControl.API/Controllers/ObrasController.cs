using ConstruControl.Application.DTOs.Obras;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos los endpoints requieren estar logueado
public class ObrasController : ControllerBase
{
    private readonly IObraRepository _obraRepository;

    public ObrasController(IObraRepository obraRepository)
    {
        _obraRepository = obraRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ObraResponseDto>>> ObtenerTodas()
    {
        var obras = await _obraRepository.ObtenerTodasAsync();
        return Ok(obras.Select(MapearADto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ObraResponseDto>> ObtenerPorId(int id)
    {
        var obra = await _obraRepository.ObtenerPorIdAsync(id);
        if (obra is null)
        {
            return NotFound(new { mensaje = $"No existe una obra con id {id}." });
        }
        return Ok(MapearADto(obra));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,JefeObra")]
    public async Task<ActionResult<ObraResponseDto>> Crear([FromBody] ObraRequestDto request)
    {
        var obra = new Obra
        {
            Nombre = request.Nombre,
            Ubicacion = request.Ubicacion,
            Presupuesto = request.Presupuesto,
            FechaInicio = request.FechaInicio,
            Estado = EstadoObra.Planificacion
        };

        await _obraRepository.AgregarAsync(obra);
        await _obraRepository.GuardarCambiosAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = obra.Id }, MapearADto(obra));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,JefeObra")]
    public async Task<ActionResult<ObraResponseDto>> Actualizar(int id, [FromBody] ObraRequestDto request)
    {
        var obra = await _obraRepository.ObtenerPorIdAsync(id);
        if (obra is null)
        {
            return NotFound(new { mensaje = $"No existe una obra con id {id}." });
        }

        obra.Nombre = request.Nombre;
        obra.Ubicacion = request.Ubicacion;
        obra.Presupuesto = request.Presupuesto;
        obra.FechaInicio = request.FechaInicio;

        _obraRepository.Actualizar(obra);
        await _obraRepository.GuardarCambiosAsync();

        return Ok(MapearADto(obra));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var obra = await _obraRepository.ObtenerPorIdAsync(id);
        if (obra is null)
        {
            return NotFound(new { mensaje = $"No existe una obra con id {id}." });
        }

        _obraRepository.EliminarLogico(obra);
        await _obraRepository.GuardarCambiosAsync();

        return NoContent();
    }

    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "Admin,JefeObra")]
    public async Task<ActionResult<ObraResponseDto>> CambiarEstado(int id, [FromBody] CambiarEstadoObraDto request)
    {
        var obra = await _obraRepository.ObtenerPorIdAsync(id);
        if (obra is null)
        {
            return NotFound(new { mensaje = $"No existe una obra con id {id}." });
        }

        if (!Enum.TryParse<EstadoObra>(request.NuevoEstado, out var nuevoEstado))
        {
            return BadRequest(new { mensaje = "Estado inválido. Use: Planificacion, Activa, Pausada, Finalizada." });
        }

        if (!obra.PuedeTransicionarA(nuevoEstado))
        {
            return BadRequest(new
            {
                mensaje = $"No se puede pasar de '{obra.Estado}' a '{nuevoEstado}'. Verifique el flujo permitido."
            });
        }

        obra.Estado = nuevoEstado;
        _obraRepository.Actualizar(obra);
        await _obraRepository.GuardarCambiosAsync();

        return Ok(MapearADto(obra));
    }

    private static ObraResponseDto MapearADto(Obra obra) => new()
    {
        Id = obra.Id,
        Nombre = obra.Nombre,
        Ubicacion = obra.Ubicacion,
        Presupuesto = obra.Presupuesto,
        FechaInicio = obra.FechaInicio,
        Estado = obra.Estado.ToString()
    };
}

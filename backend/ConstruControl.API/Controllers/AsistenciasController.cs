using ConstruControl.Application.DTOs.Asistencias;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsistenciasController : ControllerBase
{
    private readonly IAsistenciaRepository _asistenciaRepository;

    public AsistenciasController(IAsistenciaRepository asistenciaRepository)
    {
        _asistenciaRepository = asistenciaRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<AsistenciaResponseDto>>> ObtenerTodas()
    {
        var asistencias = await _asistenciaRepository.ObtenerTodasAsync();
        return Ok(asistencias.Select(MapearADto));
    }

    [HttpPost("entrada")]
    [Authorize(Roles = "Admin,JefeObra")]
    public async Task<ActionResult<AsistenciaResponseDto>> RegistrarEntrada([FromBody] RegistrarEntradaDto request)
    {
        var asistencia = await _asistenciaRepository.RegistrarEntradaAsync(request.EmpleadoId, request.ObraId);
        if (asistencia is null)
        {
            return Conflict(new { mensaje = "Ya existe una asistencia registrada hoy para este empleado en esta obra." });
        }
        return Created(string.Empty, MapearADto(asistencia));
    }

    [HttpPatch("{id}/salida")]
    [Authorize(Roles = "Admin,JefeObra")]
    public async Task<ActionResult<AsistenciaResponseDto>> RegistrarSalida(int id)
    {
        var asistencia = await _asistenciaRepository.RegistrarSalidaAsync(id);
        if (asistencia is null)
        {
            return BadRequest(new { mensaje = "No existe la asistencia, o ya tiene una salida registrada." });
        }
        return Ok(MapearADto(asistencia));
    }

    private static AsistenciaResponseDto MapearADto(Asistencia asistencia) => new()
    {
        Id = asistencia.Id,
        EmpleadoId = asistencia.EmpleadoId,
        EmpleadoNombre = asistencia.Empleado?.Nombre ?? string.Empty,
        ObraId = asistencia.ObraId,
        ObraNombre = asistencia.Obra?.Nombre ?? string.Empty,
        Fecha = asistencia.Fecha,
        HoraEntrada = asistencia.HoraEntrada,
        HoraSalida = asistencia.HoraSalida
    };
}

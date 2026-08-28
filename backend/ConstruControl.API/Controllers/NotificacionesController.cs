using ConstruControl.Application.DTOs.Notificaciones;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionRepository _notificacionRepository;

    public NotificacionesController(INotificacionRepository notificacionRepository)
    {
        _notificacionRepository = notificacionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificacionResponseDto>>> ObtenerTodas()
    {
        var notificaciones = await _notificacionRepository.ObtenerTodasAsync();
        return Ok(notificaciones.Select(MapearADto));
    }

    [HttpPatch("{id}/leida")]
    public async Task<ActionResult> MarcarLeida(int id)
    {
        var exito = await _notificacionRepository.MarcarLeidaAsync(id);
        if (!exito)
        {
            return NotFound(new { mensaje = $"No existe una notificacion con id {id}." });
        }
        return NoContent();
    }

    private static NotificacionResponseDto MapearADto(Notificacion notificacion) => new()
    {
        Id = notificacion.Id,
        Tipo = notificacion.Tipo.ToString(),
        ObraId = notificacion.ObraId,
        ObraNombre = notificacion.Obra?.Nombre,
        MaterialId = notificacion.MaterialId,
        MaterialNombre = notificacion.Material?.Nombre,
        Mensaje = notificacion.Mensaje,
        Leida = notificacion.Leida,
        FechaCreacion = notificacion.FechaCreacion
    };
}

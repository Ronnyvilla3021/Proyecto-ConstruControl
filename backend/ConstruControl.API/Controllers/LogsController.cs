using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LogsController : ControllerBase
{
    private readonly ILogRepository _logRepository;

    public LogsController(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    [HttpGet]
    public async Task<ActionResult> ObtenerTodos()
    {
        var logs = await _logRepository.ObtenerTodosAsync();
        return Ok(logs.Select(l => new
        {
            l.Id,
            UsuarioNombre = l.Usuario?.NombreCompleto,
            l.Accion,
            l.Entidad,
            l.EntidadId,
            l.Detalle,
            l.Fecha
        }));
    }
}

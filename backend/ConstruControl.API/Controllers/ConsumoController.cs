using ConstruControl.Application.DTOs.Consumo;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsumoController : ControllerBase
{
    private readonly IConsumoRepository _consumoRepository;

    public ConsumoController(IConsumoRepository consumoRepository)
    {
        _consumoRepository = consumoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ConsumoResponseDto>>> ObtenerTodos()
    {
        var consumos = await _consumoRepository.ObtenerTodosAsync();
        return Ok(consumos.Select(MapearADto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,JefeObra,Bodeguero,Empleado")]
    public async Task<ActionResult<ConsumoResponseDto>> Registrar([FromBody] ConsumoRequestDto request)
    {
        var responsableId = int.Parse(User.FindFirst("sub")!.Value);

        var consumo = await _consumoRepository.RegistrarAsync(
            request.MaterialId, request.ObraId, responsableId, request.Cantidad);

        if (consumo is null)
        {
            return BadRequest(new { mensaje = "Stock insuficiente o material inexistente para registrar este consumo." });
        }

        return Created(string.Empty, MapearADto(consumo));
    }

    private static ConsumoResponseDto MapearADto(Consumo consumo) => new()
    {
        Id = consumo.Id,
        MaterialId = consumo.MaterialId,
        MaterialNombre = consumo.Material?.Nombre ?? string.Empty,
        ObraId = consumo.ObraId,
        ObraNombre = consumo.Obra?.Nombre ?? string.Empty,
        ResponsableId = consumo.ResponsableId,
        ResponsableNombre = consumo.Responsable?.NombreCompleto ?? string.Empty,
        Cantidad = consumo.Cantidad,
        Fecha = consumo.Fecha
    };
}

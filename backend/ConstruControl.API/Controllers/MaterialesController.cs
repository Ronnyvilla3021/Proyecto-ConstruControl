using ConstruControl.Application.DTOs.Materiales;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialesController : ControllerBase
{
    private readonly IMaterialRepository _materialRepository;

    public MaterialesController(IMaterialRepository materialRepository)
    {
        _materialRepository = materialRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<MaterialResponseDto>>> ObtenerTodos()
    {
        var materiales = await _materialRepository.ObtenerTodosAsync();
        return Ok(materiales.Select(MapearADto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaterialResponseDto>> ObtenerPorId(int id)
    {
        var material = await _materialRepository.ObtenerPorIdAsync(id);
        if (material is null)
        {
            return NotFound(new { mensaje = $"No existe un material con id {id}." });
        }
        return Ok(MapearADto(material));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Bodeguero")]
    public async Task<ActionResult<MaterialResponseDto>> Crear([FromBody] MaterialRequestDto request)
    {
        var material = new Material
        {
            Nombre = request.Nombre,
            Stock = 0, // el stock inicia en 0, se incrementa por Compras
            StockMinimo = request.StockMinimo,
            Unidad = request.Unidad,
            PrecioUnitario = request.PrecioUnitario
        };

        await _materialRepository.AgregarAsync(material);
        await _materialRepository.GuardarCambiosAsync();

        return CreatedAtAction(nameof(ObtenerPorId), new { id = material.Id }, MapearADto(material));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Bodeguero")]
    public async Task<ActionResult<MaterialResponseDto>> Actualizar(int id, [FromBody] MaterialRequestDto request)
    {
        var material = await _materialRepository.ObtenerPorIdAsync(id);
        if (material is null)
        {
            return NotFound(new { mensaje = $"No existe un material con id {id}." });
        }

        material.Nombre = request.Nombre;
        material.StockMinimo = request.StockMinimo;
        material.Unidad = request.Unidad;
        material.PrecioUnitario = request.PrecioUnitario;
        // Stock NO se toca aqui - solo por Compras/Consumo

        _materialRepository.Actualizar(material);
        await _materialRepository.GuardarCambiosAsync();

        return Ok(MapearADto(material));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var material = await _materialRepository.ObtenerPorIdAsync(id);
        if (material is null)
        {
            return NotFound(new { mensaje = $"No existe un material con id {id}." });
        }

        _materialRepository.EliminarLogico(material);
        await _materialRepository.GuardarCambiosAsync();

        return NoContent();
    }

    private static MaterialResponseDto MapearADto(Material material) => new()
    {
        Id = material.Id,
        Nombre = material.Nombre,
        Stock = material.Stock,
        StockMinimo = material.StockMinimo,
        Unidad = material.Unidad,
        PrecioUnitario = material.PrecioUnitario,
        StockBajo = material.TieneStockBajo()
    };
}

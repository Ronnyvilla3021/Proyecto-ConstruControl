using ConstruControl.Application.DTOs.Facturas;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacturasController : ControllerBase
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IArchivoStorageService _storageService;

    public FacturasController(IFacturaRepository facturaRepository, IArchivoStorageService storageService)
    {
        _facturaRepository = facturaRepository;
        _storageService = storageService;
    }

    [HttpGet("compra/{compraId}")]
    public async Task<ActionResult<List<FacturaResponseDto>>> ObtenerPorCompra(int compraId)
    {
        var facturas = await _facturaRepository.ObtenerPorCompraAsync(compraId);
        return Ok(facturas.Select(MapearADto));
    }

    [HttpPost("compra/{compraId}")]
    [Authorize(Roles = "Admin,Bodeguero")]
    public async Task<ActionResult<FacturaResponseDto>> Subir(int compraId, IFormFile archivo)
    {
        if (archivo.Length == 0)
        {
            return BadRequest(new { mensaje = "El archivo esta vacio." });
        }

        await using var stream = archivo.OpenReadStream();
        var ruta = await _storageService.GuardarArchivoAsync(stream, archivo.FileName, "facturas");

        var factura = new Factura
        {
            CompraId = compraId,
            RutaArchivo = ruta
        };

        await _facturaRepository.AgregarAsync(factura);
        await _facturaRepository.GuardarCambiosAsync();

        return Created(string.Empty, MapearADto(factura));
    }

    private static FacturaResponseDto MapearADto(Factura factura) => new()
    {
        Id = factura.Id,
        CompraId = factura.CompraId,
        RutaArchivo = factura.RutaArchivo,
        FechaSubida = factura.FechaSubida
    };
}

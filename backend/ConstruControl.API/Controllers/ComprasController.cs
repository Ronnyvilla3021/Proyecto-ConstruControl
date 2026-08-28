using ConstruControl.Application.DTOs.Compras;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComprasController : ControllerBase
{
    private readonly ICompraRepository _compraRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IObraRepository _obraRepository;

    public ComprasController(
        ICompraRepository compraRepository,
        IMaterialRepository materialRepository,
        IProveedorRepository proveedorRepository,
        IObraRepository obraRepository)
    {
        _compraRepository = compraRepository;
        _materialRepository = materialRepository;
        _proveedorRepository = proveedorRepository;
        _obraRepository = obraRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompraResponseDto>>> ObtenerTodas()
    {
        var compras = await _compraRepository.ObtenerTodasAsync();
        return Ok(compras.Select(MapearADto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompraResponseDto>> ObtenerPorId(int id)
    {
        var compra = await _compraRepository.ObtenerPorIdConDetallesAsync(id);
        if (compra is null)
        {
            return NotFound(new { mensaje = $"No existe una compra con id {id}." });
        }
        return Ok(MapearADto(compra));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Bodeguero")]
    public async Task<ActionResult<CompraResponseDto>> Crear([FromBody] CompraRequestDto request)
    {
        if (request.Detalles.Count == 0)
        {
            return BadRequest(new { mensaje = "La compra debe tener al menos un material." });
        }

        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(request.ProveedorId);
        if (proveedor is null)
        {
            return BadRequest(new { mensaje = $"No existe un proveedor con id {request.ProveedorId}." });
        }

        var obra = await _obraRepository.ObtenerPorIdAsync(request.ObraId);
        if (obra is null)
        {
            return BadRequest(new { mensaje = $"No existe una obra con id {request.ObraId}." });
        }

        // UsuarioId viene del token JWT (claim "sub"), no del body -
        // asi no se puede falsificar quien crea la orden.
        var usuarioId = int.Parse(User.FindFirst("sub")!.Value);

        var compra = new Compra
        {
            ProveedorId = request.ProveedorId,
            ObraId = request.ObraId,
            UsuarioId = usuarioId,
            Estado = EstadoCompra.Pendiente,
            Fecha = DateTime.UtcNow
        };

        decimal total = 0;
        foreach (var detalleReq in request.Detalles)
        {
            var material = await _materialRepository.ObtenerPorIdAsync(detalleReq.MaterialId);
            if (material is null)
            {
                return BadRequest(new { mensaje = $"No existe un material con id {detalleReq.MaterialId}." });
            }

            var detalle = new DetalleCompra
            {
                MaterialId = detalleReq.MaterialId,
                Cantidad = detalleReq.Cantidad,
                PrecioUnitario = detalleReq.PrecioUnitario
            };
            total += detalle.Cantidad * detalle.PrecioUnitario;
            compra.Detalles.Add(detalle);
        }
        compra.Total = total;

        await _compraRepository.AgregarAsync(compra);
        await _compraRepository.GuardarCambiosAsync();

        var creada = await _compraRepository.ObtenerPorIdConDetallesAsync(compra.Id);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = compra.Id }, MapearADto(creada!));
    }

    [HttpPost("{id}/recepcion")]
    [Authorize(Roles = "Admin,Bodeguero")]
    public async Task<ActionResult> Recepcionar(int id)
    {
        var exito = await _compraRepository.RecepcionarAsync(id);
        if (!exito)
        {
            return BadRequest(new { mensaje = "La compra no existe o ya no esta en estado Pendiente." });
        }

        var compra = await _compraRepository.ObtenerPorIdConDetallesAsync(id);
        return Ok(MapearADto(compra!));
    }

    private static CompraResponseDto MapearADto(Compra compra) => new()
    {
        Id = compra.Id,
        ProveedorId = compra.ProveedorId,
        ProveedorNombre = compra.Proveedor?.Nombre ?? string.Empty,
        ObraId = compra.ObraId,
        ObraNombre = compra.Obra?.Nombre ?? string.Empty,
        Fecha = compra.Fecha,
        FechaRecepcion = compra.FechaRecepcion,
        Estado = compra.Estado.ToString(),
        Total = compra.Total,
        Detalles = compra.Detalles.Select(d => new DetalleCompraResponseDto
        {
            MaterialId = d.MaterialId,
            MaterialNombre = d.Material?.Nombre ?? string.Empty,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            Subtotal = d.Subtotal
        }).ToList()
    };
}

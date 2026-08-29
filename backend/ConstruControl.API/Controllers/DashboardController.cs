using ConstruControl.Application.DTOs.Dashboard;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ConstruControlDbContext _context;

    public DashboardController(ConstruControlDbContext context)
    {
        _context = context;
    }

    [HttpGet("{obraId}")]
    public async Task<ActionResult<DashboardResponseDto>> ObtenerDashboard(int obraId)
    {
        var obra = await _context.Obras.FirstOrDefaultAsync(o => o.Id == obraId && o.Activo);
        if (obra is null)
        {
            return NotFound(new { mensaje = $"No existe una obra con id {obraId}." });
        }

        var gastoTotal = await _context.Compras
            .Where(c => c.ObraId == obraId && c.Estado == EstadoCompra.Recibida)
            .SumAsync(c => c.Total);

        var porcentajeUsado = obra.Presupuesto > 0
            ? gastoTotal / obra.Presupuesto
            : 0;

        var diasTranscurridos = Math.Max(1, (DateTime.UtcNow.Date - obra.FechaInicio.Date).Days);
        var costoDiarioPromedio = gastoTotal / diasTranscurridos;

        // Materiales criticos: los que estan por debajo del minimo y se han
        // consumido en esta obra (relevancia para esta obra especificamente)
        var materialesUsadosEnObra = await _context.Consumos
            .Where(c => c.ObraId == obraId)
            .Select(c => c.MaterialId)
            .Distinct()
            .ToListAsync();

        var materialesCriticos = await _context.Materiales
            .Where(m => m.Activo && m.Stock <= m.StockMinimo && materialesUsadosEnObra.Contains(m.Id))
            .Select(m => new MaterialCriticoDto
            {
                MaterialId = m.Id,
                Nombre = m.Nombre,
                Stock = m.Stock,
                StockMinimo = m.StockMinimo
            })
            .ToListAsync();

        return Ok(new DashboardResponseDto
        {
            ObraId = obra.Id,
            ObraNombre = obra.Nombre,
            Presupuesto = obra.Presupuesto,
            GastoTotal = gastoTotal,
            PorcentajePresupuestoUsado = porcentajeUsado,
            DiasTranscurridos = diasTranscurridos,
            CostoDiarioPromedio = costoDiarioPromedio,
            MaterialesCriticos = materialesCriticos
        });
    }
}

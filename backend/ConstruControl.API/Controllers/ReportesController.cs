using ConstruControl.Application.DTOs.Reportes;
using ConstruControl.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstruControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet("obra/{obraId}/excel")]
    public async Task<IActionResult> DescargarExcel(int obraId, [FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
    {
        var filtro = new ReporteObraRequestDto
        {
            ObraId = obraId,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta
        };

        var bytes = await _reporteService.GenerarExcelComprasConsumosAsync(filtro);
        var nombreArchivo = $"reporte-obra-{obraId}-{DateTime.UtcNow:yyyyMMdd}.xlsx";

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
    }

    [HttpGet("obra/{obraId}/pdf")]
    public async Task<IActionResult> DescargarPdf(int obraId)
    {
        var bytes = await _reporteService.GenerarPdfIndicadoresObraAsync(obraId);
        var nombreArchivo = $"indicadores-obra-{obraId}-{DateTime.UtcNow:yyyyMMdd}.pdf";

        return File(bytes, "application/pdf", nombreArchivo);
    }
}

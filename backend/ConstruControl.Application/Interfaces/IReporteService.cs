using ConstruControl.Application.DTOs.Reportes;

namespace ConstruControl.Application.Interfaces;

public interface IReporteService
{
    /// <summary>
    /// Genera un Excel con las compras y consumos de una obra en un rango de fechas.
    /// </summary>
    Task<byte[]> GenerarExcelComprasConsumosAsync(ReporteObraRequestDto filtro);

    /// <summary>
    /// Genera un PDF con los indicadores de una obra: presupuesto, avance, costos.
    /// </summary>
    Task<byte[]> GenerarPdfIndicadoresObraAsync(int obraId);
}

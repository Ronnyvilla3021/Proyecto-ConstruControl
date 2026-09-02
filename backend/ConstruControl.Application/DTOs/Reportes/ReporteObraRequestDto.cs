namespace ConstruControl.Application.DTOs.Reportes;

public class ReporteObraRequestDto
{
    public int ObraId { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}

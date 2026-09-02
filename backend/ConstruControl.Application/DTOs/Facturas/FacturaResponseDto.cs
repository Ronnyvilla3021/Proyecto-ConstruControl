namespace ConstruControl.Application.DTOs.Facturas;

public class FacturaResponseDto
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; }
}

namespace ConstruControl.Application.DTOs.Compras;

public class CompraResponseDto
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public int ObraId { get; set; }
    public string ObraNombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public DateTime? FechaRecepcion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<DetalleCompraResponseDto> Detalles { get; set; } = new();
}

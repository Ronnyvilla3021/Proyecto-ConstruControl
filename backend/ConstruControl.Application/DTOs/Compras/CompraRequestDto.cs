namespace ConstruControl.Application.DTOs.Compras;

public class CompraRequestDto
{
    public int ProveedorId { get; set; }
    public int ObraId { get; set; }
    public List<DetalleCompraRequestDto> Detalles { get; set; } = new();
}

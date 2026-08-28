namespace ConstruControl.Application.DTOs.Compras;

public class DetalleCompraRequestDto
{
    public int MaterialId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

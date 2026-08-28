namespace ConstruControl.Application.DTOs.Compras;

public class DetalleCompraResponseDto
{
    public int MaterialId { get; set; }
    public string MaterialNombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

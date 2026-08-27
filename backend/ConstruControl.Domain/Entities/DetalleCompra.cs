namespace ConstruControl.Domain.Entities;

public class DetalleCompra
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}

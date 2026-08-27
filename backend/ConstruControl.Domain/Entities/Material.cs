namespace ConstruControl.Domain.Entities;

public class Material : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }

    // Navegación
    public ICollection<DetalleCompra> DetallesCompra { get; set; } = new List<DetalleCompra>();
    public ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();

    public bool TieneStockBajo() => Stock <= StockMinimo;
}

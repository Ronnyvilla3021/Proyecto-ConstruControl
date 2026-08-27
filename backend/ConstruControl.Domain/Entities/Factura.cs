namespace ConstruControl.Domain.Entities;

public class Factura
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}

namespace ConstruControl.Domain.Entities;

public enum EstadoCompra
{
    Pendiente,
    Recibida,
    Cancelada
}

public class Compra
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; } = null!;
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public DateTime? FechaRecepcion { get; set; }
    public EstadoCompra Estado { get; set; } = EstadoCompra.Pendiente;
    public decimal Total { get; set; }

    // Navegación
    public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}

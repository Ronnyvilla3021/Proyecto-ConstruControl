namespace ConstruControl.Domain.Entities;

public class Proveedor : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }

    // Navegación
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}

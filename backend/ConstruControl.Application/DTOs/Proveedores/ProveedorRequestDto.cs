namespace ConstruControl.Application.DTOs.Proveedores;

public class ProveedorRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}

namespace ConstruControl.Domain.Entities;

public enum RolUsuario
{
    Admin,
    JefeObra,
    Bodeguero,
    Empleado
}

public class Usuario : BaseEntity
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }

    // Navegación
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    public ICollection<Consumo> ConsumosRegistrados { get; set; } = new List<Consumo>();
}

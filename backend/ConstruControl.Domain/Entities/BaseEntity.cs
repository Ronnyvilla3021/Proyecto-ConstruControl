namespace ConstruControl.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

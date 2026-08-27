namespace ConstruControl.Domain.Entities;

public class Empleado : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }

    // Navegación
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
}

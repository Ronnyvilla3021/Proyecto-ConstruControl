namespace ConstruControl.Domain.Entities;

public class Asistencia
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public DateOnly Fecha { get; set; }
    public TimeOnly? HoraEntrada { get; set; }
    public TimeOnly? HoraSalida { get; set; }
}

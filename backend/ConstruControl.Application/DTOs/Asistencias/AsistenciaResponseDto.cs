namespace ConstruControl.Application.DTOs.Asistencias;

public class AsistenciaResponseDto
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int ObraId { get; set; }
    public string ObraNombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public TimeOnly? HoraEntrada { get; set; }
    public TimeOnly? HoraSalida { get; set; }
}

namespace ConstruControl.Application.DTOs.Empleados;

public class EmpleadoResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
}

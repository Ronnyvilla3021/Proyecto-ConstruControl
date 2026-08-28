namespace ConstruControl.Application.DTOs.Empleados;

public class EmpleadoRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
}

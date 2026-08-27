namespace ConstruControl.Application.DTOs.Obras;

public class ObraRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public decimal Presupuesto { get; set; }
    public DateTime FechaInicio { get; set; }
}

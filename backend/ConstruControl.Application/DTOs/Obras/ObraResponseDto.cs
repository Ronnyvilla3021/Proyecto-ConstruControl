namespace ConstruControl.Application.DTOs.Obras;

public class ObraResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public decimal Presupuesto { get; set; }
    public DateTime FechaInicio { get; set; }
    public string Estado { get; set; } = string.Empty;
}

namespace ConstruControl.Application.DTOs.Consumo;

public class ConsumoResponseDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public string MaterialNombre { get; set; } = string.Empty;
    public int ObraId { get; set; }
    public string ObraNombre { get; set; } = string.Empty;
    public int ResponsableId { get; set; }
    public string ResponsableNombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; }
}

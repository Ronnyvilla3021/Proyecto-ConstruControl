namespace ConstruControl.Application.DTOs.Consumo;

public class ConsumoRequestDto
{
    public int MaterialId { get; set; }
    public int ObraId { get; set; }
    public decimal Cantidad { get; set; }
}

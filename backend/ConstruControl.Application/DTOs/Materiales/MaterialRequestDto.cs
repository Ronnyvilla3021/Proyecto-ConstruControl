namespace ConstruControl.Application.DTOs.Materiales;

public class MaterialRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public decimal StockMinimo { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
}

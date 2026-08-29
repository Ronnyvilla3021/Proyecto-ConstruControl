namespace ConstruControl.Application.DTOs.Dashboard;

public class DashboardResponseDto
{
    public int ObraId { get; set; }
    public string ObraNombre { get; set; } = string.Empty;
    public decimal Presupuesto { get; set; }
    public decimal GastoTotal { get; set; }
    public decimal PorcentajePresupuestoUsado { get; set; }
    public int DiasTranscurridos { get; set; }
    public decimal CostoDiarioPromedio { get; set; }
    public List<MaterialCriticoDto> MaterialesCriticos { get; set; } = new();
}

public class MaterialCriticoDto
{
    public int MaterialId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }
}

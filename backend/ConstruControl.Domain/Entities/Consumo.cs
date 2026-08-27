namespace ConstruControl.Domain.Entities;

public class Consumo
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public int ResponsableId { get; set; }
    public Usuario Responsable { get; set; } = null!;
    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}

namespace ConstruControl.Domain.Entities;

public class FotoObra
{
    public int Id { get; set; }
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}

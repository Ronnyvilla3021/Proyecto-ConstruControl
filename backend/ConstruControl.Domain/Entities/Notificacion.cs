namespace ConstruControl.Domain.Entities;

public enum TipoNotificacion
{
    StockBajo,
    PresupuestoExcedido,
    CompraSugerida
}

public class Notificacion
{
    public int Id { get; set; }
    public TipoNotificacion Tipo { get; set; }
    public int? ObraId { get; set; }
    public Obra? Obra { get; set; }
    public int? MaterialId { get; set; }
    public Material? Material { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

namespace ConstruControl.Application.DTOs.Notificaciones;

public class NotificacionResponseDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int? ObraId { get; set; }
    public string? ObraNombre { get; set; }
    public int? MaterialId { get; set; }
    public string? MaterialNombre { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; }
    public DateTime FechaCreacion { get; set; }
}

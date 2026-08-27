namespace ConstruControl.Application.DTOs.Obras;

public class CambiarEstadoObraDto
{
    public string NuevoEstado { get; set; } = string.Empty; // "Planificacion","Activa","Pausada","Finalizada"
}

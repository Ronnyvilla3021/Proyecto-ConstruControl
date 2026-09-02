namespace ConstruControl.Application.DTOs.FotosObra;

public class FotoObraResponseDto
{
    public int Id { get; set; }
    public int ObraId { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaSubida { get; set; }
}

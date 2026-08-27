namespace ConstruControl.Domain.Entities;

public class Log
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string Accion { get; set; } = string.Empty;   // "Crear", "Actualizar", "Eliminar"
    public string Entidad { get; set; } = string.Empty;  // "Obra", "Material", "Compra"...
    public int? EntidadId { get; set; }
    public string? Detalle { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}

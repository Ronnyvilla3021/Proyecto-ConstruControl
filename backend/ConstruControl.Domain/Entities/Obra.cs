namespace ConstruControl.Domain.Entities;

public enum EstadoObra
{
    Planificacion,
    Activa,
    Pausada,
    Finalizada
}

public class Obra : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public decimal Presupuesto { get; set; }
    public DateTime FechaInicio { get; set; }
    public EstadoObra Estado { get; set; } = EstadoObra.Planificacion;

    // Navegación
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    public ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    public ICollection<FotoObra> Fotos { get; set; } = new List<FotoObra>();

    /// <summary>
    /// Valida si la transición de estado es permitida según el flujo de negocio:
    /// Planificacion -> Activa -> (Pausada &lt;-&gt; Activa) -> Finalizada
    /// </summary>
    public bool PuedeTransicionarA(EstadoObra nuevoEstado)
    {
        return (Estado, nuevoEstado) switch
        {
            (EstadoObra.Planificacion, EstadoObra.Activa) => true,
            (EstadoObra.Activa, EstadoObra.Pausada) => true,
            (EstadoObra.Activa, EstadoObra.Finalizada) => true,
            (EstadoObra.Pausada, EstadoObra.Activa) => true,
            (EstadoObra.Pausada, EstadoObra.Finalizada) => true,
            _ => false
        };
    }
}

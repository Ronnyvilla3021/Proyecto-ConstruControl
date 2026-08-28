using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IAsistenciaRepository
{
    Task<List<Asistencia>> ObtenerTodasAsync();
    Task<Asistencia?> RegistrarEntradaAsync(int empleadoId, int obraId);
    Task<Asistencia?> RegistrarSalidaAsync(int asistenciaId);
}

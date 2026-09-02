using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface ILogRepository
{
    Task<List<Log>> ObtenerTodosAsync();
    Task RegistrarAsync(int? usuarioId, string accion, string entidad, int? entidadId, string? detalle);
}

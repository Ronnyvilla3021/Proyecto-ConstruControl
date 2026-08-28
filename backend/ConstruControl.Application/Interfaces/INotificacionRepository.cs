using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface INotificacionRepository
{
    Task<List<Notificacion>> ObtenerTodasAsync();
    Task<bool> ExisteNoLeidaAsync(TipoNotificacion tipo, int? obraId, int? materialId);
    Task AgregarAsync(Notificacion notificacion);
    Task<bool> MarcarLeidaAsync(int id);
    Task GuardarCambiosAsync();
}

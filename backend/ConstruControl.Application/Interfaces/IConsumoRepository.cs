using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IConsumoRepository
{
    Task<List<Consumo>> ObtenerTodosAsync();

    /// <summary>
    /// Registra un consumo y descuenta el stock del material, todo en una
    /// transaccion. Devuelve null si no hay stock suficiente.
    /// </summary>
    Task<Consumo?> RegistrarAsync(int materialId, int obraId, int responsableId, decimal cantidad);
}

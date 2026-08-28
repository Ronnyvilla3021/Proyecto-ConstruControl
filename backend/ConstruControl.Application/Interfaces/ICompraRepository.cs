using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface ICompraRepository
{
    Task<List<Compra>> ObtenerTodasAsync();
    Task<Compra?> ObtenerPorIdConDetallesAsync(int id);
    Task AgregarAsync(Compra compra);
    Task GuardarCambiosAsync();

    /// <summary>
    /// Ejecuta la recepcion de una compra dentro de una transaccion:
    /// marca la compra como Recibida e incrementa el stock de cada material.
    /// </summary>
    Task<bool> RecepcionarAsync(int compraId);
}

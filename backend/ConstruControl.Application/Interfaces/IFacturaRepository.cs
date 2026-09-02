using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IFacturaRepository
{
    Task<List<Factura>> ObtenerPorCompraAsync(int compraId);
    Task AgregarAsync(Factura factura);
    Task GuardarCambiosAsync();
}

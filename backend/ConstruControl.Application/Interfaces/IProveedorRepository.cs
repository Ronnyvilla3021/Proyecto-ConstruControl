using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IProveedorRepository
{
    Task<List<Proveedor>> ObtenerTodosAsync();
    Task<Proveedor?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Proveedor proveedor);
    void Actualizar(Proveedor proveedor);
    void EliminarLogico(Proveedor proveedor);
    Task GuardarCambiosAsync();
}

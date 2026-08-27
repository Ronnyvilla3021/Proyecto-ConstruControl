using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IMaterialRepository
{
    Task<List<Material>> ObtenerTodosAsync();
    Task<Material?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Material material);
    void Actualizar(Material material);
    void EliminarLogico(Material material);
    Task GuardarCambiosAsync();
}

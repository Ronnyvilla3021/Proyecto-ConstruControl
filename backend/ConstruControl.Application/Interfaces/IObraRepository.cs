using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IObraRepository
{
    Task<List<Obra>> ObtenerTodasAsync();
    Task<Obra?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Obra obra);
    void Actualizar(Obra obra);
    void EliminarLogico(Obra obra);
    Task GuardarCambiosAsync();
}

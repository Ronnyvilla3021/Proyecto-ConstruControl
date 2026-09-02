using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IFotoObraRepository
{
    Task<List<FotoObra>> ObtenerPorObraAsync(int obraId);
    Task AgregarAsync(FotoObra foto);
    Task GuardarCambiosAsync();
}

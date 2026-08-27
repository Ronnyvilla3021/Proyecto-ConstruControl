using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<bool> ExisteEmailAsync(string email);
    Task<bool> ExisteAlgunUsuarioAsync();
    Task AgregarAsync(Usuario usuario);
    Task GuardarCambiosAsync();
}

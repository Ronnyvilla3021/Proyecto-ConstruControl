using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IJwtService
{
    string GenerarToken(Usuario usuario);
}

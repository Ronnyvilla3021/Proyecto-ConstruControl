using ConstruControl.Domain.Entities;

namespace ConstruControl.Application.Interfaces;

public interface IEmpleadoRepository
{
    Task<List<Empleado>> ObtenerTodosAsync();
    Task<Empleado?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Empleado empleado);
    void Actualizar(Empleado empleado);
    void EliminarLogico(Empleado empleado);
    Task GuardarCambiosAsync();
}

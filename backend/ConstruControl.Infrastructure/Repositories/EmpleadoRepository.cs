using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly ConstruControlDbContext _context;

    public EmpleadoRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Empleado>> ObtenerTodosAsync()
    {
        return await _context.Empleados
            .Where(e => e.Activo)
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    public async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    public async Task AgregarAsync(Empleado empleado)
    {
        await _context.Empleados.AddAsync(empleado);
    }

    public void Actualizar(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
    }

    public void EliminarLogico(Empleado empleado)
    {
        empleado.Activo = false;
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}

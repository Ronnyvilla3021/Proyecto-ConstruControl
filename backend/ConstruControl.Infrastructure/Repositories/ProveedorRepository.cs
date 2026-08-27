using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly ConstruControlDbContext _context;

    public ProveedorRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Proveedor>> ObtenerTodosAsync()
    {
        return await _context.Proveedores
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(int id)
    {
        return await _context.Proveedores
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);
    }

    public async Task AgregarAsync(Proveedor proveedor)
    {
        await _context.Proveedores.AddAsync(proveedor);
    }

    public void Actualizar(Proveedor proveedor)
    {
        _context.Proveedores.Update(proveedor);
    }

    public void EliminarLogico(Proveedor proveedor)
    {
        proveedor.Activo = false;
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}

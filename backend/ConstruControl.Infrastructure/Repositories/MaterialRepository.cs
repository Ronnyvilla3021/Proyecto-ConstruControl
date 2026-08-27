using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly ConstruControlDbContext _context;

    public MaterialRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Material>> ObtenerTodosAsync()
    {
        return await _context.Materiales
            .Where(m => m.Activo)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Material?> ObtenerPorIdAsync(int id)
    {
        return await _context.Materiales
            .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
    }

    public async Task AgregarAsync(Material material)
    {
        await _context.Materiales.AddAsync(material);
    }

    public void Actualizar(Material material)
    {
        _context.Materiales.Update(material);
    }

    public void EliminarLogico(Material material)
    {
        material.Activo = false;
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}

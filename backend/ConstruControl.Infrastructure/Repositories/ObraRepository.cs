using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class ObraRepository : IObraRepository
{
    private readonly ConstruControlDbContext _context;

    public ObraRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Obra>> ObtenerTodasAsync()
    {
        return await _context.Obras
            .Where(o => o.Activo)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Obra?> ObtenerPorIdAsync(int id)
    {
        return await _context.Obras
            .FirstOrDefaultAsync(o => o.Id == id && o.Activo);
    }

    public async Task AgregarAsync(Obra obra)
    {
        await _context.Obras.AddAsync(obra);
    }

    public void Actualizar(Obra obra)
    {
        _context.Obras.Update(obra);
    }

    public void EliminarLogico(Obra obra)
    {
        obra.Activo = false;
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}

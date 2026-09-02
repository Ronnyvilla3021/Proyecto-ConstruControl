using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class FotoObraRepository : IFotoObraRepository
{
    private readonly ConstruControlDbContext _context;

    public FotoObraRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<FotoObra>> ObtenerPorObraAsync(int obraId)
    {
        return await _context.FotosObra
            .Where(f => f.ObraId == obraId)
            .OrderByDescending(f => f.FechaSubida)
            .ToListAsync();
    }

    public async Task AgregarAsync(FotoObra foto)
    {
        await _context.FotosObra.AddAsync(foto);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}

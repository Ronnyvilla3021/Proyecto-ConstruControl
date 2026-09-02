using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class FacturaRepository : IFacturaRepository
{
    private readonly ConstruControlDbContext _context;

    public FacturaRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Factura>> ObtenerPorCompraAsync(int compraId)
    {
        return await _context.Facturas
            .Where(f => f.CompraId == compraId)
            .OrderByDescending(f => f.FechaSubida)
            .ToListAsync();
    }

    public async Task AgregarAsync(Factura factura)
    {
        await _context.Facturas.AddAsync(factura);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}

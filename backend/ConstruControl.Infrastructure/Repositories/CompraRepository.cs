using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class CompraRepository : ICompraRepository
{
    private readonly ConstruControlDbContext _context;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public CompraRepository(ConstruControlDbContext context, IRealtimeNotifier realtimeNotifier)
    {
        _context = context;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<List<Compra>> ObtenerTodasAsync()
    {
        return await _context.Compras
            .Include(c => c.Proveedor)
            .Include(c => c.Obra)
            .Include(c => c.Detalles)
                .ThenInclude(d => d.Material)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
    }

    public async Task<Compra?> ObtenerPorIdConDetallesAsync(int id)
    {
        return await _context.Compras
            .Include(c => c.Proveedor)
            .Include(c => c.Obra)
            .Include(c => c.Detalles)
                .ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AgregarAsync(Compra compra)
    {
        await _context.Compras.AddAsync(compra);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RecepcionarAsync(int compraId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var compra = await _context.Compras
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Material)
                .FirstOrDefaultAsync(c => c.Id == compraId);

            if (compra is null || compra.Estado != EstadoCompra.Pendiente)
            {
                return false;
            }

            foreach (var detalle in compra.Detalles)
            {
                detalle.Material.Stock += detalle.Cantidad;
            }

            compra.Estado = EstadoCompra.Recibida;
            compra.FechaRecepcion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Fuera de la transaccion (ya confirmada) - notifica a los clientes conectados
            await _realtimeNotifier.NotificarActualizacionDashboardAsync(
                compra.ObraId,
                "CompraRecepcionada",
                new { compraId = compra.Id, total = compra.Total });

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

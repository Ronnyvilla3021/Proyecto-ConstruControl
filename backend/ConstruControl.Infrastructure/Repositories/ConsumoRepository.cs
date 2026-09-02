using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class ConsumoRepository : IConsumoRepository
{
    private readonly ConstruControlDbContext _context;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public ConsumoRepository(ConstruControlDbContext context, IRealtimeNotifier realtimeNotifier)
    {
        _context = context;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<List<Consumo>> ObtenerTodosAsync()
    {
        return await _context.Consumos
            .Include(c => c.Material)
            .Include(c => c.Obra)
            .Include(c => c.Responsable)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
    }

    public async Task<Consumo?> RegistrarAsync(int materialId, int obraId, int responsableId, decimal cantidad)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var material = await _context.Materiales
                .FirstOrDefaultAsync(m => m.Id == materialId && m.Activo);

            if (material is null || material.Stock < cantidad)
            {
                return null;
            }

            material.Stock -= cantidad;

            var consumo = new Consumo
            {
                MaterialId = materialId,
                ObraId = obraId,
                ResponsableId = responsableId,
                Cantidad = cantidad,
                Fecha = DateTime.UtcNow
            };

            await _context.Consumos.AddAsync(consumo);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var consumoCompleto = await _context.Consumos
                .Include(c => c.Material)
                .Include(c => c.Obra)
                .Include(c => c.Responsable)
                .FirstAsync(c => c.Id == consumo.Id);

            await _realtimeNotifier.NotificarActualizacionDashboardAsync(
                obraId,
                "ConsumoRegistrado",
                new { materialNombre = material.Nombre, cantidad, stockRestante = material.Stock });

            return consumoCompleto;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

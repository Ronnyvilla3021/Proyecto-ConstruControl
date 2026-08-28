using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class NotificacionRepository : INotificacionRepository
{
    private readonly ConstruControlDbContext _context;

    public NotificacionRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notificacion>> ObtenerTodasAsync()
    {
        return await _context.Notificaciones
            .Include(n => n.Obra)
            .Include(n => n.Material)
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();
    }

    /// <summary>
    /// Evita generar notificaciones duplicadas: si ya existe una sin leer
    /// del mismo tipo para la misma obra/material, no se crea otra.
    /// </summary>
    public async Task<bool> ExisteNoLeidaAsync(TipoNotificacion tipo, int? obraId, int? materialId)
    {
        return await _context.Notificaciones.AnyAsync(n =>
            n.Tipo == tipo &&
            n.ObraId == obraId &&
            n.MaterialId == materialId &&
            !n.Leida);
    }

    public async Task AgregarAsync(Notificacion notificacion)
    {
        await _context.Notificaciones.AddAsync(notificacion);
    }

    public async Task<bool> MarcarLeidaAsync(int id)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id);
        if (notificacion is null)
        {
            return false;
        }
        notificacion.Leida = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}

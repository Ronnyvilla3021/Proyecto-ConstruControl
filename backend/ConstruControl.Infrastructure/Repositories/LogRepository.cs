using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class LogRepository : ILogRepository
{
    private readonly ConstruControlDbContext _context;

    public LogRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Log>> ObtenerTodosAsync()
    {
        return await _context.Logs
            .Include(l => l.Usuario)
            .OrderByDescending(l => l.Fecha)
            .Take(200) // evita traer un historial infinito de una sola vez
            .ToListAsync();
    }

    public async Task RegistrarAsync(int? usuarioId, string accion, string entidad, int? entidadId, string? detalle)
    {
        await _context.Logs.AddAsync(new Log
        {
            UsuarioId = usuarioId,
            Accion = accion,
            Entidad = entidad,
            EntidadId = entidadId,
            Detalle = detalle
        });
        await _context.SaveChangesAsync();
    }
}

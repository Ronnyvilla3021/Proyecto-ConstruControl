using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Repositories;

public class AsistenciaRepository : IAsistenciaRepository
{
    private readonly ConstruControlDbContext _context;

    public AsistenciaRepository(ConstruControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asistencia>> ObtenerTodasAsync()
    {
        return await _context.Asistencias
            .Include(a => a.Empleado)
            .Include(a => a.Obra)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }

    public async Task<Asistencia?> RegistrarEntradaAsync(int empleadoId, int obraId)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        var yaExiste = await _context.Asistencias.AnyAsync(a =>
            a.EmpleadoId == empleadoId && a.ObraId == obraId && a.Fecha == hoy);

        if (yaExiste)
        {
            return null;
        }

        var asistencia = new Asistencia
        {
            EmpleadoId = empleadoId,
            ObraId = obraId,
            Fecha = hoy,
            HoraEntrada = TimeOnly.FromDateTime(DateTime.UtcNow)
        };

        await _context.Asistencias.AddAsync(asistencia);
        await _context.SaveChangesAsync();

        return await _context.Asistencias
            .Include(a => a.Empleado)
            .Include(a => a.Obra)
            .FirstAsync(a => a.Id == asistencia.Id);
    }

    public async Task<Asistencia?> RegistrarSalidaAsync(int asistenciaId)
    {
        var asistencia = await _context.Asistencias
            .Include(a => a.Empleado)
            .Include(a => a.Obra)
            .FirstOrDefaultAsync(a => a.Id == asistenciaId);

        if (asistencia is null || asistencia.HoraSalida is not null)
        {
            return null;
        }

        asistencia.HoraSalida = TimeOnly.FromDateTime(DateTime.UtcNow);
        await _context.SaveChangesAsync();

        return asistencia;
    }
}

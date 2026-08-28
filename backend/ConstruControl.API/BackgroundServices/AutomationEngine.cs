using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.API.BackgroundServices;

/// <summary>
/// Motor de automatizacion: revisa periodicamente el sistema y genera
/// notificaciones de stock bajo, presupuesto excedido y compra sugerida.
/// </summary>
public class AutomationEngine : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutomationEngine> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(15);

    public AutomationEngine(IServiceScopeFactory scopeFactory, ILogger<AutomationEngine> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AutomationEngine iniciado. Intervalo de revision: {Intervalo}", Intervalo);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EjecutarCicloAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                // Un fallo en el ciclo no debe tumbar el servicio - se loguea y se reintenta en el siguiente ciclo.
                _logger.LogError(ex, "Error durante el ciclo de automatizacion.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    private async Task EjecutarCicloAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando ciclo de automatizacion: {Fecha}", DateTime.UtcNow);

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ConstruControlDbContext>();
        var notificacionRepo = scope.ServiceProvider.GetRequiredService<INotificacionRepository>();

        await RevisarStockBajoAsync(context, notificacionRepo);
        await RevisarPresupuestoExcedidoAsync(context, notificacionRepo);

        await notificacionRepo.GuardarCambiosAsync();

        _logger.LogInformation("Ciclo de automatizacion completado.");
    }

    private async Task RevisarStockBajoAsync(ConstruControlDbContext context, INotificacionRepository notificacionRepo)
    {
        var materialesConStockBajo = await context.Materiales
            .Where(m => m.Activo && m.Stock <= m.StockMinimo)
            .ToListAsync();

        foreach (var material in materialesConStockBajo)
        {
            var yaNotificado = await notificacionRepo.ExisteNoLeidaAsync(
                TipoNotificacion.StockBajo, null, material.Id);

            if (!yaNotificado)
            {
                await notificacionRepo.AgregarAsync(new Notificacion
                {
                    Tipo = TipoNotificacion.StockBajo,
                    MaterialId = material.Id,
                    Mensaje = $"Stock bajo: '{material.Nombre}' tiene {material.Stock} {material.Unidad} " +
                              $"(minimo: {material.StockMinimo})."
                });

                // Compra sugerida: mismo material, mensaje distinto
                await notificacionRepo.AgregarAsync(new Notificacion
                {
                    Tipo = TipoNotificacion.CompraSugerida,
                    MaterialId = material.Id,
                    Mensaje = $"Se sugiere generar una orden de compra para '{material.Nombre}' " +
                              $"(stock actual: {material.Stock} {material.Unidad})."
                });

                _logger.LogWarning("Stock bajo detectado: {Material} ({Stock}/{Minimo})",
                    material.Nombre, material.Stock, material.StockMinimo);
            }
        }
    }

    private async Task RevisarPresupuestoExcedidoAsync(ConstruControlDbContext context, INotificacionRepository notificacionRepo)
    {
        const decimal UmbralAlerta = 0.90m; // 90% del presupuesto

        var obrasActivas = await context.Obras
            .Where(o => o.Activo && (o.Estado == EstadoObra.Activa || o.Estado == EstadoObra.Pausada))
            .ToListAsync();

        foreach (var obra in obrasActivas)
        {
            var gastoTotal = await context.Compras
                .Where(c => c.ObraId == obra.Id && c.Estado == EstadoCompra.Recibida)
                .SumAsync(c => c.Total);

            if (obra.Presupuesto <= 0)
            {
                continue;
            }

            var porcentajeUsado = gastoTotal / obra.Presupuesto;

            if (porcentajeUsado >= UmbralAlerta)
            {
                var yaNotificado = await notificacionRepo.ExisteNoLeidaAsync(
                    TipoNotificacion.PresupuestoExcedido, obra.Id, null);

                if (!yaNotificado)
                {
                    await notificacionRepo.AgregarAsync(new Notificacion
                    {
                        Tipo = TipoNotificacion.PresupuestoExcedido,
                        ObraId = obra.Id,
                        Mensaje = $"La obra '{obra.Nombre}' ha usado el {porcentajeUsado:P1} de su presupuesto " +
                                  $"(${gastoTotal:N2} de ${obra.Presupuesto:N2})."
                    });

                    _logger.LogWarning("Presupuesto excedido en obra {Obra}: {Porcentaje:P1}",
                        obra.Nombre, porcentajeUsado);
                }
            }
        }
    }
}

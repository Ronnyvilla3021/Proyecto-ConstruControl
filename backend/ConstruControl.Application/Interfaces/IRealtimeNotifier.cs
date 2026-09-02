namespace ConstruControl.Application.Interfaces;

/// <summary>
/// Abstraccion para notificar eventos en tiempo real a los clientes conectados
/// al dashboard de una obra. La implementacion real (SignalR) vive en la capa API,
/// asi Infrastructure no depende de detalles de transporte en tiempo real.
/// </summary>
public interface IRealtimeNotifier
{
    Task NotificarActualizacionDashboardAsync(int obraId, string evento, object datos);
}

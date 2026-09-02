using ConstruControl.API.Hubs;
using ConstruControl.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ConstruControl.API.Realtime;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<DashboardHub> _hubContext;

    public SignalRRealtimeNotifier(IHubContext<DashboardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotificarActualizacionDashboardAsync(int obraId, string evento, object datos)
    {
        await _hubContext.Clients
            .Group(DashboardHub.GrupoDeObra(obraId.ToString()))
            .SendAsync(evento, datos);
    }
}

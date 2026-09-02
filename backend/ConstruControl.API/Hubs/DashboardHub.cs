using Microsoft.AspNetCore.SignalR;

namespace ConstruControl.API.Hubs;

/// <summary>
/// Hub de tiempo real para el dashboard. Los clientes se unen al grupo de
/// una obra especifica y reciben eventos cuando hay cambios relevantes
/// (compra recepcionada, consumo registrado, nueva notificacion).
/// </summary>
public class DashboardHub : Hub
{
    public async Task JoinObraGroup(string obraId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GrupoDeObra(obraId));
    }

    public async Task LeaveObraGroup(string obraId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GrupoDeObra(obraId));
    }

    public static string GrupoDeObra(string obraId) => $"obra-{obraId}";
}

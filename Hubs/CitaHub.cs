using Microsoft.AspNetCore.SignalR;

namespace PoliclinicoWeb.Hubs
{
    public class CitaHub : Hub
    {
        // Hub kept intentionally minimal — server will push events: CitaCreada, CitaActualizada, CitaEliminada
    }
}

using Microsoft.AspNetCore.SignalR;

namespace Restaurant.Hubs
{
    public class OrderHub : Hub
    {
        public async Task JoinKitchenGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "KitchenGroup");
            await Clients.Caller.SendAsync("ConnectionEstablished", Context.ConnectionId);
        }

        public async Task LeaveKitchenGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "KitchenGroup");
        }

        public async Task RequestStatusUpdate(int orderId)
        {
            // Could trigger a database lookup and status push
            await Clients.Caller.SendAsync("StatusRequested", orderId);
        }
    }
}
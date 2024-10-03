using Microsoft.AspNetCore.SignalR;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.Hub
{
    public class OrderHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SendOrderUpdate(OrderDto order)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", order);
        }
    }
}

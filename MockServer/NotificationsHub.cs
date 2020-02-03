using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MockServer
{
    public class NotificationsHub : Hub
    {
        public async Task OnConnected(string userID)
        {
            await Groups.AddAsync(Context.ConnectionId, userID);
        }

        public async Task OnDisconnected(string userID)
        {
            await Groups.RemoveAsync(Context.ConnectionId, userID);
        }
    }
}

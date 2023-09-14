using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// To show online users in real time
namespace API.SignalR
{
    [Authorize] // Only authorized users can connect to this hub (through web sockets) - similar to Http
    public class UserPresenceHub : Hub
    {

        // Override methods 

        // When a user connects to the hub 
        public override async Task OnConnectedAsync()
        {
            // Clients object -> invoke methods on clients connected to hub
            
            // User connects to the OnlineUser method. Whoever connects to this method, those connected receive the username of the user who connected to the hub (which is the Getusername method)
            await Clients.Others.SendAsync("OnlineUser", Context.User.GetUsername());
        }

        public override async Task OnDisconnectedAsync(Exception exp)
        {
            // removes the user from the list of online users
            await Clients.Others.SendAsync("OfflineUser", Context.User.GetUsername());

            // calls the base disconnect method (since we pass an exception)
            await base.OnDisconnectedAsync(exp);
        }

    }
}
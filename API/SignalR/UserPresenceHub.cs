using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// To show online users in real time (UserPresenceTracker is used to keep track of online users)

namespace API.SignalR
{
    [Authorize] // Only authorized users can connect to this hub (through web sockets) - similar to Http
    public class UserPresenceHub : Hub
    {
        private readonly UserPresenceTracker _userPresenceTracker;

        public UserPresenceHub(UserPresenceTracker userPresenceTracker)
        {
            _userPresenceTracker = userPresenceTracker;

        }


        // Override methods 

        // When a user connects to the hub 
        public override async Task OnConnectedAsync()
        {
            // Clients object -> invoke methods on clients connected to hub

            // When a user connects to the hub, we want to add them to the dictionary of online users + return a bool if they are connecting for the first time (so online)
           var userOnline =  await _userPresenceTracker.ConnectUser(Context.User.GetUsername(), Context.ConnectionId);

            // User connects to the OnlineUser method. Whoever connects to this method, those connected receive the username of the user who connected to the hub (which is the Getusername method)
            if (userOnline) // if the user is connecting for the first time -> We notify users 
            await Clients.Others.SendAsync("OnlineUser", Context.User.GetUsername());

            // Get the list of online users
            var usersCurrentlyOnline = await _userPresenceTracker.GetUsersOnline();

            // Send all connected users to the caller (including the user who connected to the hub)
            await Clients.Caller.SendAsync("GetUsersCurrentlyOnline", usersCurrentlyOnline); 
            

        }

        // When a user disconnects from the hub
        public override async Task OnDisconnectedAsync(Exception exp)
        {
            // removes the user from the dictionary of online users
         var userOffline = await _userPresenceTracker.DisconnectUser(Context.User.GetUsername(), Context.ConnectionId);

            // removes the user from the list of online users
            if (userOffline) // read onConnectedAsync comments ^
            await Clients.Others.SendAsync("OfflineUser", Context.User.GetUsername());

            // Get the list of online users after the user disconnects
            var usersCurrentlyOnline = await _userPresenceTracker.GetUsersOnline(); 

            // calls the base disconnect method (since we pass an exception)
            await base.OnDisconnectedAsync(exp);


        }




    }
}
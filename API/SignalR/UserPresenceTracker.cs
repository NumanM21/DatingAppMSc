using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


//TODO: Currently using Dictionary to store online users. This is not scalable. 
// Implement Redis to store online users. Redis is a key-value store that is very fast and can be used to store online users- it is scalable!!

namespace API.SignalR
{
    public class UserPresenceTracker
    {
        // key -> username, value -> connection id of the user
        // value is a list because a user can be connected to the hub from multiple devices (multiple connection ids)
        private static readonly Dictionary<string, List<string>> UsersOnline = new Dictionary<string, List<string>>();

        // Add a user to the dictionary with the connection id
        public Task ConnectUser(string username, string idConnection)
        {
            // lock the dictionary so that only one thread can access it at a time -> dict is not thread safe otherwise
            lock(UsersOnline)
            {
                if (UsersOnline.ContainsKey(username))
                {
                    UsersOnline[username].Add(idConnection); 
                }
                else // don't have a key for this user
                {
                    UsersOnline.Add(username, new List<string>{idConnection});
                } 
            }

            // return a completed task 
            return Task.CompletedTask; 
        }

        // Remove a user from the dictionary (when they disconnect)
        public Task DiconnectUser(string username, string idConnection)
        {
            lock(UsersOnline)
            {
                // check if the user is in the dictionary
                if (UsersOnline.ContainsKey(username))
                {
                    // disconnect from this connection id
                    UsersOnline[username].Remove(idConnection); 

                    if (UsersOnline[username].Count == 0)
                    {
                        // remove the user from the dictionary if they have no more connection ids
                        UsersOnline.Remove(username); 
                    }
                }
            }
            return Task.CompletedTask;
        }


        // Get all online users -> return a list of usernames -> Other users can see who is online
        public Task<string[]> GetUsersOnline()
        {
            string[] arrUsersOnline;

            lock (UsersOnline)
            {
                // OrderBy to sort the usernames alphabetically
                // Select -> Return the key (username) of each item in the dictionary
                arrUsersOnline = UsersOnline.OrderBy(x=> x.Key).Select(x=>x.Key).ToArray();
            }
            // return the array of usernames
            return Task.FromResult(arrUsersOnline);
        }

        // Get all connections for A user //TODO: Use Redis instead of Dictionary to store online users --> More scalable!
        public static Task<List<string>> UserConnectionsGetter(string username)
        {
            // list to store the connections id (meaning our notification is sent to ALL their devices)
            List<string> allUserConnectionsIds;

            lock(UsersOnline)
            {
                // Return list of connection ids for this user
                allUserConnectionsIds = UsersOnline.GetValueOrDefault(username); 
            }

            // return the list of connection ids
            return Task.FromResult(allUserConnectionsIds);
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class UserMessageHub : Hub
    {
        // Once user connects to message hub, want to return thread between users
        private readonly IMessageUserRepository _userMessageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _userMapper;
        private readonly IHubContext<UserPresenceHub> _userPresenceHub;

        public UserMessageHub(IMapper userMapper, IUserRepository userRepo, IMessageUserRepository userMessageRepo, IHubContext<UserPresenceHub> userPresenceHub)
        {
            _userPresenceHub = userPresenceHub; // can access other hubs from this hub (via injection)
            _userMapper = userMapper;
            _userRepo = userRepo;
            _userMessageRepo = userMessageRepo;
        }


        // Override methods from UserPresenceHub

        public override async Task OnConnectedAsync()
        {
            // when we click 'msg' button on profile, we are connected to that user

            // get http context from hub context  -> can send query string from client to server
            var contextHttp = Context.GetHttpContext();
            var connectingToUser = contextHttp.Request.Query["user"];

            // need to put users in a group (group name is alphabetical order of usernames)
            var nameOfGroup = GroupNameGetter(Context.User.GetUsername(), connectingToUser);

            // user connects to group
            await Groups.AddToGroupAsync(Context.ConnectionId, nameOfGroup);

            // add group to DB
            await GroupToAdd(nameOfGroup);

            // get messages between users (curr user and user they are connecting to)
            var msgBetweenUsers = await _userMessageRepo.LoadMessageBetweenUsers(Context.User.GetUsername(), connectingToUser);

            // send messages to group (signalR returns msg to all users in group, not specific api call)
            await Clients.Group(nameOfGroup).SendAsync("LoadMessageBetweenUsers", msgBetweenUsers);
        }


        private string GroupNameGetter(string userCalling, string otherUser)
        {
            var alphabeticalString = string.CompareOrdinal(userCalling, otherUser) < 0; // <0 so returns bool

            // returns 0 if equal, -1 if first string is less than second string, 1 if first string is greater than second string

            return alphabeticalString ? $"{userCalling}-{otherUser}" : $"{otherUser}-{userCalling}";
        }

        // user disconnects from hub (all groups) when they click away from message page

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // remove user from group
            await MsgGroupRemover();

            await base.OnDisconnectedAsync(exception);
        }

        // similar to messagecreate method in MessageUserController
        public async Task MessageSender(MessageCreateDto msgCreateDto)
        {
            // get user from context hub
            var username = Context.User.GetUsername();

            // check if message being sent to self
            if (username == msgCreateDto.messageReceivingUsername.ToLower())
            {   // Exception more resource heavy than http response
                throw new HubException("message cannot be sent to self");
            }

            // get sender username
            var senderUser = await _userRepo.AsyncGetUserByUsername(username);

            // get receiving user username
            var receivingUser = await _userRepo.AsyncGetUserByUsername(msgCreateDto.messageReceivingUsername);

            // check if receiving user exists
            if (receivingUser == null)
            {
                throw new HubException("User not found");
            }

            // create new message
            var msg = new MessageUser
            {
                SenderUser = senderUser,
                messageSenderUsername = senderUser.UserName,
                ReceivingUser = receivingUser,
                messageReceivingUsername = receivingUser.UserName,
                messageContent = msgCreateDto.messageContent
            };

            // get group name
            var nameOfGroup = GroupNameGetter(senderUser.UserName, receivingUser.UserName);

            // get group from repo
            var msgGroup = await _userMessageRepo.GroupMsgGetter(nameOfGroup);

            // check connection and see if our user matches the receiving user --> if so, set date read to now 
            if (msgGroup.GroupConnections.Any(u => u.Username == receivingUser.UserName))
            {
                msg.messageReadAt = DateTime.UtcNow;
            }
            else // Not in the same message group as my sending user
            {
                // get connection from repo for receiving user
                var userConnection = await UserPresenceTracker.UserConnectionsGetter(receivingUser.UserName);

                // if user connection exists, will have a connection id
                if (userConnection != null)
                {
                    // Since user is online, we can send them a notification (via SignalR) that they have a new message
                    
                    //FIXME: URL redirect erro rosmehwere here
                    await _userPresenceHub.Clients.Clients(userConnection).SendAsync("ReceiveNewMessage", new {username = senderUser.UserName, knownAs = senderUser.KnownAs });
                }
            }

            // add message to DB
            _userMessageRepo.MessageAdd(msg);

            // Save changes --> return new message to other users in group
            if (await _userMessageRepo.AsyncSaveAll())
            {
                // send message to group
                await Clients.Group(nameOfGroup).SendAsync("SendNewMessage", _userMapper.Map<MessageUserDto>(msg));
            }
        }

        // Add group to DB
        private async Task<bool> GroupToAdd(string nameOfGroup)
        {
            // get message group from msg repo
            var msgGroup = await _userMessageRepo.GroupMsgGetter(nameOfGroup);

            // Create new connection 
            var newConnection = new SRGroupConnection(Context.ConnectionId, Context.User.GetUsername());

            // check if group exists (null if group doesn't exist)
            if (msgGroup == null)
            {
                // create new group
                msgGroup = new SignalRGroup(nameOfGroup);

                // Add group to repo
                _userMessageRepo.GroupAdd(msgGroup);
            }
            else  //TODO: Change this to try catch (more efficient?)
            {
                // check if connection already exists
                var connectionExists = msgGroup.GroupConnections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

                // if connection exists, return false
                if (connectionExists != null)
                {
                    return false;
                }
            }

            // Create new connection 
            msgGroup.GroupConnections.Add(newConnection);

            // save changes (saveAll returns bool for method header)
            return await _userMessageRepo.AsyncSaveAll();

        }

        // Remove connection from DB (Have another method ^above to remove user from group once they disconnect from hub)
        private async Task MsgGroupRemover()
        {
            // get connection from repo
            var connection = await _userMessageRepo.ConnectionGetter(Context.ConnectionId);

            // remove connection from repo
            _userMessageRepo.ConnectionRemove(connection);

            // save changes
            await _userMessageRepo.AsyncSaveAll();
        }

    }
}
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
using Microsoft.VisualBasic;

namespace API.SignalR
{
    [Authorize]
    public class UserMessageHub : Hub
    {
        // Once user connects to message hub, want to return thread between users
        private readonly IMapper _userMapper;
        private readonly IHubContext<UserPresenceHub> _userPresenceHub;
        private readonly IUnitOfWork _unitOfWork;

        public UserMessageHub(IMapper userMapper, IUnitOfWork unitOfWork, IHubContext<UserPresenceHub> userPresenceHub)
        {
            _unitOfWork = unitOfWork;
            _userPresenceHub = userPresenceHub; // can access other hubs from this hub (via injection)
            _userMapper = userMapper;

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

            // add group to DB + return group
            var addedGroup = await GroupToAdd(nameOfGroup);

            // Client receives this and can update their group
            await Clients.Group(nameOfGroup).SendAsync("GroupUpdate", addedGroup);

            // get messages between users (curr user and user they are connecting to)
            var msgBetweenUsers = await _unitOfWork.RepositoryMessageUser.LoadMessageBetweenUsers(Context.User.GetUsername(), connectingToUser);

            // Check if there are changes (new message) --> if so, we save changes (to DB) --> Can now use unit of work to track changes made and save in oneplace

            if (_unitOfWork.ContainsChanges()) await _unitOfWork.TransactionComplete(); 

            // send messages to caller -> caller is the user who connected to the hub --> So they get the message thread
            await Clients.Caller.SendAsync("LoadMessageBetweenUsers", msgBetweenUsers);
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
            var removeGroup = await MsgGroupRemover();

            // Send the group we want to remove to our client -> can update group
            await Clients.Group(removeGroup.Name).SendAsync("GroupUpdate", removeGroup);

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
            var senderUser = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(username);

            // get receiving user username
            var receivingUser = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(msgCreateDto.messageReceivingUsername);

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
            var msgGroup = await _unitOfWork.RepositoryMessageUser.GroupMsgGetter(nameOfGroup);

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

                    await _userPresenceHub.Clients.Clients(userConnection).SendAsync("ReceiveNewMessage", new { username = senderUser.UserName, knownAs = senderUser.KnownAs });
                }
            }

            // add message to DB
            _unitOfWork.RepositoryMessageUser.MessageAdd(msg);

            // Save changes --> return new message to other users in group
            if (await _unitOfWork.TransactionComplete())
            {
                // send message to group
                await Clients.Group(nameOfGroup).SendAsync("SendNewMessage", _userMapper.Map<MessageUserDto>(msg));
            }
        }

        // Add group to DB
        private async Task<SignalRGroup> GroupToAdd(string nameOfGroup)
        {
            // get message group from msg repo
            var msgGroup = await _unitOfWork.RepositoryMessageUser.GroupMsgGetter(nameOfGroup);

            // Create new connection 
            var newConnection = new SRGroupConnection(Context.ConnectionId, Context.User.GetUsername());

            // check if group exists (null if group doesn't exist)
            if (msgGroup == null)
            {
                // create new group
                msgGroup = new SignalRGroup(nameOfGroup);

                // Add group to repo
                _unitOfWork.RepositoryMessageUser.GroupAdd(msgGroup);
            }
            else  //TODO: Change this to try catch (more efficient?)
            {
                // check if connection already exists
                var connectionExists = msgGroup.GroupConnections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

                // if connection exists, return group 
                if (connectionExists != null)
                {
                    return msgGroup;
                }
            }

            // Create new connection 
            msgGroup.GroupConnections.Add(newConnection);

            // save changes (--> if successful, return group)
            if (await _unitOfWork.TransactionComplete())
                return msgGroup;

            else throw new HubException("Group failed to be added");

        }

        // Remove connection from DB (Have another method ^above to remove user from group once they disconnect from hub)
        private async Task<SignalRGroup> MsgGroupRemover()
        {
            // get connection group using connection id
            var groupConnected = await _unitOfWork.RepositoryMessageUser.GroupConnectionGetter(Context.ConnectionId);

            // now we get connction of THIS specific group
            var connect = groupConnected.GroupConnections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);

            // remove connection from repo
            _unitOfWork.RepositoryMessageUser.ConnectionRemove(connect);

            // save changes
            if (await _unitOfWork.TransactionComplete())
                return groupConnected;

            else throw new HubException("Connection could not be removed from group");

        }

    }
}
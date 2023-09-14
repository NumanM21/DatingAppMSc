using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class UserMessageHub : Hub
    {
        // Once user connects to message hub, want to return thread between users
        private readonly IMessageUserRepository _userMessageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _userMapper;

        public UserMessageHub(IMapper userMapper, IUserRepository userRepo, IMessageUserRepository userMessageRepo)
        {
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

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
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

            // add message to DB
            _userMessageRepo.MessageAdd(msg);

            // Save changes --> return new message to other users in group
            if (await _userMessageRepo.AsyncSaveAll())
            {
                // get group name to send to
                var nameOfGroup = GroupNameGetter(senderUser.UserName, receivingUser.UserName);

                // send message to group
                await Clients.Group(nameOfGroup).SendAsync("NewMessage", _userMapper.Map<MessageUserDto>(msg));
            }


        }

    }
}
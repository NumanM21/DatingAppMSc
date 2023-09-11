
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class MessageUserController : BaseApiController
  {
    private readonly IMessageUserRepository _repoMessageUser;
    private readonly IUserRepository _repoUser;
    private readonly IMapper _map;

    public MessageUserController(IUserRepository repoUser, IMessageUserRepository repoMessageUser, IMapper map)
    {
      _map = map;
      _repoUser = repoUser;
      _repoMessageUser = repoMessageUser;

    }

    // Create a message
    [HttpPost]
    public async Task<ActionResult<MessageUserDto>> MessageCreate(MessageCreateDto messageCreateDto)
    {
        // get username (from token - claimsprincipleextension)
        var username = User.GetUsername();

        // check if message being sent to self (cannot send message to self)

        if (username == messageCreateDto.messageReceivingUsername.ToLower()) 
        {
            return BadRequest("Message cannot be sent to self");
        }

        // get username of sender
        var senderUser = await _repoUser.AsyncGetUserByUsername(username);

        // get username of receiving user -> From client
        var receivingUser = await _repoUser.AsyncGetUserByUsername(messageCreateDto.messageReceivingUsername);

        // check if receiving user exists
        if (receivingUser == null) return NotFound("User not found");

        // create new message
        var msg = new MessageUser
        {
            SenderUser = senderUser,
            messageSenderUsername = senderUser.UserName,
            ReceivingUser = receivingUser,
            messageReceivingUsername = receivingUser.UserName,
            messageContent = messageCreateDto.messageContent // message content from client (hence use a Dto)
        };

        // add message to DB (need to use context .add so EF tracks our message)
        _repoMessageUser.MessageAdd(msg); 


        if (await _repoMessageUser.AsyncSaveAll()) return Ok(_map.Map<MessageUserDto>(msg)); 

        return BadRequest("Message not sent");


    }

  }
}
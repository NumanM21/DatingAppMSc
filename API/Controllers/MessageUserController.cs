
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.ExternalHelpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class MessageUserController : BaseApiController
  {

    private readonly IMapper _map;
    private readonly IUnitOfWork _unitOfWork;

    public MessageUserController(IUnitOfWork unitOfWork, IMapper map)
    {
      _unitOfWork = unitOfWork;
      _map = map;
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
      var senderUser = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(username);

      // get username of receiving user -> From client
      var receivingUser = await _unitOfWork.RepositoryUser.AsyncGetUserByUsername(messageCreateDto.messageReceivingUsername);

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
      _unitOfWork.RepositoryMessageUser.MessageAdd(msg);


      if (await _unitOfWork.TransactionComplete()) return Ok(_map.Map<MessageUserDto>(msg));

      return BadRequest("Message not sent");
    }

    // Get message for a user
    [HttpGet]
    public async Task<ActionResult<PaginationList<MessageUserDto>>> LoadMessageForUser([FromQuery] ParameterMessage parameterMessage)
    {

      // get username (from token - claimsprincipleextension)
      parameterMessage.currUsername = User.GetUsername();

      // get messages for user
      var msg = await _unitOfWork.RepositoryMessageUser.LoadMessageForUser(parameterMessage);

      // add pagination header to response
      Response.HeadPaginationAdd(new HeadPagination
      (
      msg.currentPage,
      msg.PageSize,
      msg.totalCount,
      msg.totalPage
      ));

      // return messages
      return msg;
    }

    // Get message between two users -> Don't need API anymore
    // [HttpGet("message-between-users/{username}")]
    // public async Task<ActionResult<IEnumerable<MessageUserDto>>> LoadMessageBetweenUsers(string username)
    // {
    //   // get curr username
    //   var currUsername = User.GetUsername();

    //   // return messages between two users
    //   return Ok(await _unitOfWork.RepositoryMessageUser.LoadMessageBetweenUsers(currUsername, username)); // username comes from route HttpGet

    // }

    // Delete message -> Only delete message once both users have deleted it
    [HttpDelete("{messageId}")]
    public async Task<ActionResult> messageDelete(int messageId)
    {
      // get username
      var username = User.GetUsername();

      // get message from repo
      var msg = await _unitOfWork.RepositoryMessageUser.MessageGetter(messageId);

      if (msg == null)
      {
        return NotFound("Message not found");
      }

      // check if user deleting message is the sender or receiver

      if (msg.messageSenderUsername != username && msg.messageReceivingUsername != username) return Unauthorized("You cannot delete this message");

      if (msg.messageSenderUsername == username) msg.messageSentDeleted = true; // sender is deleting msg
      
      if (msg.messageReceivingUsername == username) msg.messageReceivingDeleted = true; // receiver is deleting msg

      // check if both users have deleted message

      if (msg.messageSentDeleted && msg.messageReceivingDeleted) _unitOfWork.RepositoryMessageUser.MessageDelete(msg);

      return (await _unitOfWork.TransactionComplete()) ? Ok() : BadRequest("Message cannot be deleted");

    }

  }
}
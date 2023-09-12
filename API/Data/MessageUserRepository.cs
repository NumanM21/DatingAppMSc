
using API.DTOs;
using API.Entities;
using API.ExternalHelpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class MessageUserRepository : IMessageUserRepository
  {
    private readonly DataContext _context;
    private readonly IMapper _map;

    public MessageUserRepository(DataContext context, IMapper map)
    {
      _map = map;
      _context = context;
    }


    public async Task<bool> AsyncSaveAll()
    {
      return await _context.SaveChangesAsync() > 0; // if more than 0 changes, return true
    }

    public async Task<IEnumerable<MessageUserDto>> LoadMessageBetweenUsers(string currUsername, string receivingUsername)
    {
      // get messages between two users (and their photos)
      var msg = await _context.Message
      .Include(x => x.SenderUser).ThenInclude(x => x.Photos)
      .Include(x => x.ReceivingUser).ThenInclude(x => x.Photos)
      .Where(
        x => x.messageReceivingUsername == currUsername && x.messageReceivingDeleted == false && 
        x.messageSenderUsername == receivingUsername
      // check both ways
      || x.messageReceivingUsername == receivingUsername
      && x.messageSentDeleted == false && x.messageSenderUsername == currUsername)
      // order by message sent at (newest first)
      .OrderBy(x => x.messageSentAt).ToListAsync();
      // Now we have the entities

      // mark unread messages as sent
      var messageUnread = msg.Where(x => x.messageReadAt == null && x.messageReceivingUsername == currUsername).ToList();
      // ToList since above already gets it from DB (hence no async)

      // if there are unread messages (mark them)
      if (messageUnread.Any())
      {
        foreach (var uMsg in messageUnread)
        {
          uMsg.messageReadAt = System.DateTime.UtcNow;
        }
        // need to save changes to DB
        await _context.SaveChangesAsync();
      }

      // map our message to  message user dto

      return _map.Map<IEnumerable<MessageUserDto>>(msg);


    }

    public async Task<PaginationList<MessageUserDto>> LoadMessageForUser(ParameterMessage parameterMessage)
    {
      // store our query in a variable
      var query = _context.Message.OrderByDescending(m => m.messageSentAt)
      .AsQueryable();

      // switch statement to determine which messages container to return

      query = parameterMessage.messageContainer
      switch
      {

        "Inbox" => query.Where(x => x.messageReceivingUsername == parameterMessage.currUsername
        && x.messageReceivingDeleted == false), // check if message is deleted

        "Sent" => query.Where(x => x.messageSenderUsername == parameterMessage.currUsername
        && x.messageSentDeleted == false), 

        // default case (message not read hence check null)

        _ => query.Where(x => x.messageReceivingUsername == parameterMessage.currUsername && x.messageReceivingDeleted == false && x.messageReadAt == null)

      };

      // map our query to a list of message user dto

      var msg = query.ProjectTo<MessageUserDto>(_map.ConfigurationProvider);

      // return our paginated list of message user dto

      return await PaginationList<MessageUserDto>.AsyncCreate(msg, parameterMessage.PageNumber, parameterMessage.PageSize);

    }

    public void MessageAdd(MessageUser message)
    {
      _context.Message.Add(message);
    }

    public void MessageDelete(MessageUser message)
    {
      _context.Message.Remove(message);
    }

    public async Task<MessageUser> MessageGetter(int messageId)
    {
      return await _context.Message.FindAsync(messageId);
    }
  }
}
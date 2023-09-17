
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

    // Look interface for comments!
    public async Task<SRGroupConnection> ConnectionGetter(string connectionId)
    {
      return await _context.GroupConnection.FindAsync(connectionId);
    }

    public void ConnectionRemove(SRGroupConnection srGroupConnection)
    {
      _context.GroupConnection.Remove(srGroupConnection);
    }

    public void GroupAdd(SignalRGroup groupSr)
    {
      _context.GroupSignalR.Add(groupSr);
    }

    public async Task<SignalRGroup> GroupConnectionGetter(string connectionId)
    {
      return await _context.GroupSignalR
      .Include(g => g.GroupConnections) // we want to include group connections
      .Where(g => g.GroupConnections // where group connections contains connection id equal to connection id passed in
      .Any(x => x.ConnectionId == connectionId)).FirstOrDefaultAsync(); // if there is a connection id, return the first group
    }

    public async Task<SignalRGroup> GroupMsgGetter(string groupName)
    {
      // Goes through groupSignalR, includes groupConnections, gets first group by name

      return await _context.GroupSignalR
      .Include(sr => sr.GroupConnections)
      .FirstOrDefaultAsync(sr => sr.Name == groupName);
    }

    public async Task<IEnumerable<MessageUserDto>> LoadMessageBetweenUsers(string currUsername, string receivingUsername)
    {
      // get messages between two users (and their photos) -> use projection 
      var msgQuery =  _context.Message
      .Where(
        x => x.messageReceivingUsername == currUsername && x.messageReceivingDeleted == false &&
        x.messageSenderUsername == receivingUsername
      // check both ways
      || x.messageReceivingUsername == receivingUsername
      && x.messageSentDeleted == false && x.messageSenderUsername == currUsername)
      // order by message sent at (newest first)
      .OrderBy(x => x.messageSentAt).AsQueryable();

      // mark unread messages as sent
      var messageUnread = msgQuery.Where(x => x.messageReadAt == null && x.messageReceivingUsername == currUsername).ToList();
      // ToList since above already gets it from DB (hence no async)

      // if there are unread messages (mark them)
      if (messageUnread.Any())
      {
        foreach (var uMsg in messageUnread)
        {
          uMsg.messageReadAt = System.DateTime.UtcNow; // AutoMapper will map this to messageReadAt in date time
        }
      }

      // return our query as a list of message user dto
      return await msgQuery.ProjectTo<MessageUserDto>(_map.ConfigurationProvider).ToListAsync();


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
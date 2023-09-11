
using API.DTOs;
using API.Entities;
using API.ExternalHelpers;
using API.Interfaces;

namespace API.Data
{
  public class MessageUserRepository : IMessageUserRepository
  {
    private readonly DataContext _context;

    public MessageUserRepository(DataContext context)
    {
      _context = context;
        
    }
    public async Task<bool> AsyncSaveAll()
    {
      return await _context.SaveChangesAsync() > 0; // if more than 0 changes, return true
    }

    public Task<IEnumerable<MessageUserDto>> LoadMessageBetweenUsers(int sourceUserId, int receivingUserId)
    {
       throw new NotImplementedException();
    }

    public Task<PaginationList<MessageUserDto>> LoadMessageForUser()
    {
      throw new NotImplementedException();
    }

    public void MessageAdd(MessageUser message)
    {
      _context.Message.Add(message);
    }

    public void MessageDelete(MessageUser message)
    {
      _context.Message.Remove(message);
    }

    public async Task<MessageUser> AsyncMessageGetter(int messageId)
    {
      return await _context.Message.FindAsync(messageId);
    }
  }
}
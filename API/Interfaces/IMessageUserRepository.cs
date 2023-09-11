
using API.DTOs;
using API.Entities;
using API.ExternalHelpers;

namespace API.Interfaces
{
    public interface IMessageUserRepository
    {
    
        void MessageAdd(MessageUser message);
        void MessageDelete(MessageUser message);
        Task<MessageUser> AsyncMessageGetter(int messageId);
        Task<PaginationList<MessageUserDto>> LoadMessageForUser();        
        Task<IEnumerable<MessageUserDto>> LoadMessageBetweenUsers(int sourceUserId, int receivingUserId);

        Task<bool> AsyncSaveAll();
    }
}

using API.DTOs;
using API.Entities;
using API.ExternalHelpers;

namespace API.Interfaces
{
    public interface IMessageUserRepository
    {
    
        void MessageAdd(MessageUser message);
        void MessageDelete(MessageUser message);
        Task<MessageUser> MessageGetter(int messageId);
        Task<PaginationList<MessageUserDto>> LoadMessageForUser(ParameterMessage parameterMessage);        
        Task<IEnumerable<MessageUserDto>> LoadMessageBetweenUsers(string currUsername, string receivingUsername);

        Task<bool> AsyncSaveAll();

        // methods to track user connections to groups
        void GroupAdd(SignalRGroup groupSr); // add group to DB
        void ConnectionRemove(SRGroupConnection srGroupConnection); // remove connection from group
        Task<SRGroupConnection> ConnectionGetter(string connectionId); // get connection  by name

        Task<SignalRGroup> GroupMsgGetter(string groupName); // get group by name

        Task<SignalRGroup> GroupConnectionGetter(string connectionId); // get group by connection id

    }
}
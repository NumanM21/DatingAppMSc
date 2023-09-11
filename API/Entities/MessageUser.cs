
namespace API.Entities
{
    public class MessageUser
    {
        public int messageId { get; set; } 
        public int messageSenderId { get; set; }
        public string messageSenderUsername { get; set; }
        public AppUser SenderUser { get; set; } // related enitity to above
        public int messageReceivingId { get; set; }    
        public string  messageReceivingUsername { get; set; }
        public AppUser ReceivingUser { get; set; }
        public string messageContent { get; set; }
        public DateTime? messageReadAt { get; set; }
        public DateTime messageSentAt { get; set; } = DateTime.UtcNow;
        
        // Both bool have have to be true to delete message from DB (otherwise, who ever deletes cannot see it, but the other can)
        public bool messageSentDeleted { get; set; }
        public bool messageReceivingDeleted { get; set; }
    }
}
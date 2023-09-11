
namespace API.DTOs
{
    public class MessageUserDto
    {
         public int messageId { get; set; } 
        public int messageSenderId { get; set; }
        public string messageSenderUsername { get; set; }
        public string messageSenderPhotoURL { get; set; }
        public int messageReceivingId { get; set; }    
        public string  messageReceivingUsername { get; set; }
        public string messageReceivingPhotoURL { get; set; }
        public string messageContent { get; set; }
        public DateTime? messageReadAt { get; set; }
        public DateTime messageSentAt { get; set; } 
       
        // public bool messageSentDeleted { get; set; }
        // public bool messageReceivingDeleted { get; set; }
        
    }
}
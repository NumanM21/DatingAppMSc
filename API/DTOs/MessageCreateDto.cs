

namespace API.DTOs
{
    public class MessageCreateDto
    {
        // username of receiving user (who are we sending message to)
        public string messageReceivingUsername { get; set; }
        public string messageContent { get; set; }
        
    }
}
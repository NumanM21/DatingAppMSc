 
namespace API.ExternalHelpers
{
    public class ParameterMessage : ParameterPagination
    {
        // username of the user that is logged in
        public string currUsername { get; set; }
        public string messageContainer { get; set; } = "Unread"; // default message to return is unread
        
    }
}

namespace API.ExternalHelpers
{
    public class ParameterLikes : ParameterPagination
    {
        // Additional properties we want (we already get all properties from ParameterPagination)

        public int userId { get; set; }
        public string predicate { get; set; }
        
    }
}
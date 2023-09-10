
// Take parameters from client so we can use them in our query (for pagination).
namespace API.ExternalHelpers
{
    public class ParameterFromUser : ParameterPagination
    {   
        // We need to know who the current user is (so we can exclude them from the list of users we return)
        public string currUsername { get; set; }

        // order by filter (default returns users by last active)
        public string orderByActive { get; set; }

        // gender filter (default returns opposite sex)
        public string gender { get; set; }

        // age filter 
        public int minAge  { get; set; } = 18; // legal age we allow
        public int maxAge { get; set; } = 150; 
    }
}

// Take parameters from client so we can use them in our query (for pagination).
namespace API.ExternalHelpers
{
    public class ParameterFromUser
    {   
        // Users can 'filter' to how many users they want to see per page, but we will have a max
        // TODO: Can create different size of normal and premium users (if we have time to implement that)
        private const int maxPageSize = 30; 
        public int PageNumber { get; set; }  = 1; // default to page 1
        private int _pageSize = 5; // default to 5 users per page
        public int PageSize // return this in IUserRepository.cs
        {
            get => _pageSize; // returns value of 5
            // User can 'request' to see a 'value' of users, if value > than 30 (max) we return our 30 (so we don't crash our app)!
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value; 
        }
        
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
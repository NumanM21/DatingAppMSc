

// Used for joining between AppUser and Roles table

using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUserWithRoles : IdentityUserRole<int>
    {
        public AppUser appUser { get; set; }
        public Roles appRole { get; set; }
        
    }
}
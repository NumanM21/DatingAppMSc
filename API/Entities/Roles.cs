
using Microsoft.AspNetCore.Identity;

// Want many to many relationship between appUser and application role
// each role can have many users, and each user can have many roles (admin can also be normal member)

namespace API.Entities
{
    public class Roles : IdentityRole<int>
    {
        // property to the join table to app user (in RolesInAppUser)
        public ICollection<AppUserWithRoles> RolesUser { get; set; }
        
    }
}
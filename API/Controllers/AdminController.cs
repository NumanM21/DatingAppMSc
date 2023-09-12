

using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
    private readonly UserManager<AppUser> _managerUser;

        public AdminController(UserManager<AppUser> managerUser)
        {
         _managerUser = managerUser;  
        }

        [Authorize(Policy = "AdminRoleRequired")]
        [HttpGet("app-users-with-roles")]
        // Want to get list of users with their roles
        public async Task<ActionResult> GetAppUserWithRoles()
        {
            // Get all users (.Users table in database and order by username alphabetically)
            var user = await _managerUser.Users.OrderBy(x=>x.UserName)
            // Want to get roles (Access user roles from user , then user roles to get roles)
            // FIXME: Array of role not showing up
            .Select(x => new {
                x.Id, //userID
                Username = x.UserName,
                Roles = x.AppUserRoles.Select(s => s.appRole.Name).ToList() // List of roles 
            }).ToListAsync();

             // return list of users with their roles
            return Ok(user);

        }

        [Authorize(Policy = "ModeratorRoleRequired")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotoToModerate()
        {
            return Ok("Admin or Moderator can see this");
        }
        
    }
}
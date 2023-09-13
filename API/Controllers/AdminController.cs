

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

        // Admin can edit roles

        [Authorize(Policy = "AdminRoleRequired")]
        [HttpPost("admin-edit-roles/{username}")] //maybe httpPut? 
        public async Task<ActionResult> AdminRoleEdit(string username, [FromQuery] string newRoles)
        // httpPost {} and [FromQuery] {} are both parameters (variable names must == postman variable names) KEY!!
        // newRoles will be a comma separated string of roles (list)
        {
            // See if we have anything in the query string
            if (string.IsNullOrEmpty(newRoles)) return BadRequest("No roles were selected");

            // split the string into an array
            var newRolesSelected = newRoles.Split(",").ToArray();

            // get user we want to modify roles for
            var currUser = await _managerUser.FindByNameAsync(username);

            // Getting from root parameter ({username}), not token so HAVE to check if user exists
            if (currUser == null) return NotFound("User not found");

            // Get current role(s) for user
            var currRoles = await _managerUser.GetRolesAsync(currUser);

            // Add new roles to user
            var res = await _managerUser.AddToRolesAsync(currUser, newRolesSelected.Except(currRoles));

            // if adding roles fails
            if (res.Succeeded == false) return BadRequest("Roles could not be added");

            // Remove roles from user (roles user already had but not in newRolesSelected)
            var removedRes = await _managerUser.RemoveFromRolesAsync(currUser, currRoles.Except(newRolesSelected));

            if (removedRes.Succeeded == false) return BadRequest("Current roles could not be removed");

            // return list of roles for user (so we can update client)
            return Ok(await _managerUser.GetRolesAsync(currUser));

        }
        
    }
}
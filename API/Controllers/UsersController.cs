using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// These are all endpoints 

namespace API.Controllers
{
  [Authorize] // Wherever we place this, need to authorize user to access all endpoints after it (our authorization is jwt token)
  public class UsersController : BaseApiController
  {
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
      _context = context;

    }
    [AllowAnonymous]
    [HttpGet] // Api end point (this + route make up our API root)
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() // gets all users
    {
      var users = await _context.Users.ToListAsync();

      return users;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUser(int id) // get individual user
    {
      return await _context.Users.FindAsync(id);
    }

  }
}

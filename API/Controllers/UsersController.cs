using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

  [ApiController]
  [Route("api/[controller]")] // GET /api/users (how to access this controller)

  public class UsersController : ControllerBase
  {

    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
      _context = context;

    }

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

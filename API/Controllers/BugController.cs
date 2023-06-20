using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class BugController : BaseApiController
  {
    private readonly DataContext _context;
    public BugController(DataContext context)
    {
      _context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
      return "secrete text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
      var random = _context.Users.Find(-1); // find returns null if no user found

      if (random == null) return NotFound();

      return random;
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetSeverError()
    {
        var random = _context.Users.Find(-1);

        var randomToReturn = random.ToString(); // exception if we .ToString a 'null'

        return randomToReturn;
    }
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
      return BadRequest("Not a good request");
    }

  }
}
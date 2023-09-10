using API.ExternalHelpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(UserActiveLogger))]  // this will be applied to all endpoints in this controller 
    [ApiController]
    [Route("api/[controller]")] // GET /api/users (how to access this controller)

    public class BaseApiController : ControllerBase
    {
        
    }
}
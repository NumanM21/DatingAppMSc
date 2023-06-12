using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")] // GET /api/users (how to access this controller)

    public class BaseApiController : ControllerBase
    {
        
    }
}
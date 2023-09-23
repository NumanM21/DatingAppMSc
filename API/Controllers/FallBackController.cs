using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallBackController : Controller
    // API makes angular handle rooting
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML"); // this will return the index.html file from the wwwroot folder -> if we refresh and we are on a page that doesn't exist
        }
        
    }
}
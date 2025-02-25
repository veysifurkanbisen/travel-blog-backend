using Microsoft.AspNetCore.Mvc;

namespace TravelBlog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello World from Travel Blog API!" });
        }
    }
}
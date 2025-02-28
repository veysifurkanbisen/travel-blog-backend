using Microsoft.AspNetCore.Mvc;

namespace TravelBlog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Get()
        {
            var response = Ok(new { message = "Hello World from Travel Blog API!" });
            return response;
        }
    }
}
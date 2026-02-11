using Microsoft.AspNetCore.Mvc;

namespace RuangKampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("RuangKampus API hidup 🔥");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace dotnet_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            Log.Information("Received a ping request.");
            return Ok("Pong from .NET service");
        }

        [HttpGet("error")]
        public IActionResult ErrorTest()
        {
            try
            {
                throw new Exception("This is a test exception");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during /error call.");
                return StatusCode(500, "Error occurred");
            }
        }
    }
}

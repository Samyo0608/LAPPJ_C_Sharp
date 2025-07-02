using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    // 這邊是API控制器的特性，必須加上這個特性才能讓ASP.NET Core知道這是API控制器
    [ApiController]
    // 這邊是路由的特性，這個特性會讓ASP.NET Core知道這個控制器的路由是什麼
    [Route("api/[controller]")]

    public class HomeController(ILogger<HomeController> logger) : ControllerBase
    {
        private readonly ILogger<HomeController> _logger = logger;

        // /api/home
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "LAPPJ API server is running",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                version = "1.0.0"
            });
        }

        // /api/home/status
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            return Ok(new
            {
                status = "healthy",
                serverName = "LAPPJ C# API",
                environment = environmentName,
            });
        }

        // /api/home/test
        [HttpPost("test")]
        public IActionResult PostTest([FromQuery] TestRequest Request)
        {
            if (Request == null)
            { 
                return BadRequest(new
                {
                    error = "Request cannot be null",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            _logger.LogInformation("收到測試請求: {Message} ", Request.Message);

            return Ok(new
            {
                received = Request.Message,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                echo = $"Server received: {Request.Message}"
            });
        }
    }

    public class TestRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}

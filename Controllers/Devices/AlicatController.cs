using backend.Services.Devices;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Devices
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlicatController(IAlicatService alicatService, ILogger<AlicatController> logger) : ControllerBase
    {
        private readonly IAlicatService _alicatService = alicatService;
        private readonly ILogger<AlicatController> _logger = logger;

        // /api/alicat/read
        [HttpGet("read")]
        public async Task<IActionResult> Read()
        {
            try
            {
                var reading = await _alicatService.ReadAsync();
                return Ok(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "讀取Alicat資料失敗: {Message}", ex.Message);
                return StatusCode(500, "伺服器錯誤");
            }
        }
    }
}
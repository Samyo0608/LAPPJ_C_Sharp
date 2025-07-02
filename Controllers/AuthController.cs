using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Auth;
using backend.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
  {
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterRequest request)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

          return BadRequest(ApiResponse<UserDto>.ErrorResult("資料驗證失敗", errors));
        }

        var result = await _authService.RegisterAsync(request);

        if (result.Success)
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "註冊失敗: {Message}", ex.Message);
        return StatusCode(500, ApiResponse<UserDto>.ErrorResult("伺服器錯誤"));
      }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

          return BadRequest(ApiResponse<AuthResponse>.ErrorResult("資料驗證失敗", errors));
        }

        var result = await _authService.LoginAsync(request);

        if (result.Success)
        {
          return Ok(result);
        }

        return Unauthorized(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "登入失敗: {Message}", ex.Message);
        return StatusCode(500, ApiResponse<AuthResponse>.ErrorResult("伺服器錯誤"));
      }
    }

    [HttpPost("refresh")]
    [Authorize] // 確保使用者已登入
    public async Task<ActionResult<ApiResponse<string>>> RefreshToken()
    {
      try
      {
        var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
          return Unauthorized(ApiResponse<string>.ErrorResult("無效的使用者 ID"));
        }

        var result = await _authService.RefreshTokenAsync(userId);

        if (result.Success)
        {
          return Ok(result);
        }

        return Unauthorized(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "刷新 Token 失敗: {Message}", ex.Message);
        return StatusCode(500, ApiResponse<string>.ErrorResult("伺服器錯誤"));
      }
    }

    [HttpPost("photo")]
    [Authorize] // 確保使用者已登入
    public async Task<ActionResult<ApiResponse<string>>> UploadPhoto([FromForm] PhotoUploadRequest request)
    {
      try
      {
        var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
          return Unauthorized(ApiResponse<string>.ErrorResult("無效的使用者 ID"));
        }

        var result = await _authService.UploadPhotoAsync(userId, request);

        if (result.Success)
        {
          return Ok(result);
        }

        return BadRequest(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "上傳照片失敗: {Message}", ex.Message);
        return StatusCode(500, ApiResponse<string>.ErrorResult("伺服器錯誤"));
      }
    }

    public class PhotoUploadRequest
    {
      [Required(ErrorMessage = "照片資料不能空白。")]
      public string PhotoBase64 { get; set; } = string.Empty;

      [Required(ErrorMessage = "照片檔案名稱不能空白。")]
      public string FileName { get; set; } = string.Empty;
    }
  }
}
  // 注意：這裡的 IAuthService 和 ApiResponse<UserDto> 是假設已經存在的服務和 DTO}

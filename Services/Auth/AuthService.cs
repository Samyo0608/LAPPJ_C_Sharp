using backend.DTOs.Auth;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using backend.Infrastructure.Data;
using static backend.Controllers.AuthController;

namespace backend.Services.Auth
{
  public interface IAuthService
  {
    Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<string>> RefreshTokenAsync(int userId);
    Task<ApiResponse<string>> UploadPhotoAsync(int userId, PhotoUploadRequest request);
  }

  public class AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger) : IAuthService
  {
    private readonly AppDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request)
    {
      try
      {
        // 檢查用戶名是否已存在
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null)
        {
          return ApiResponse<UserDto>.ErrorResult("此名稱已經存在");
        }

        // 檢查 Email 是否已存在
        var existingEmail = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingEmail != null)
        {
          return ApiResponse<UserDto>.ErrorResult("此 Email 已經存在");
        }

        // 建立新使用者
        var user = new User
        {
          Username = request.Username,
          Email = request.Email,
          PasswordHash = HashPassword(request.Password),
          CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
          Id = user.Id,
          Username = user.Username,
          Email = user.Email,
          CreatedAt = user.CreatedAt
        };

        return ApiResponse<UserDto>.SuccessResult(userDto, "註冊成功");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "註冊使用者時發生錯誤");
        return ApiResponse<UserDto>.ErrorResult("註冊失敗");
      }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
      try
      {
        // 從DbContext內的Users資料表中查找使用者
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
          return ApiResponse<AuthResponse>.ErrorResult("名稱或密碼錯誤");
        }

        // 生成 JWT Token
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var authResponse = new AuthResponse
        {
          Token = token,
          RefreshToken = refreshToken,
          User = new UserDto
          {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PhotoPath = user.PhotoPath,
            SavePath = user.SavePath,
            CreatedAt = user.CreatedAt
          }
        };

        return ApiResponse<AuthResponse>.SuccessResult(authResponse, "登入成功");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "使用者登入時發生錯誤");
        return ApiResponse<AuthResponse>.ErrorResult("登入失敗");
      }
    }

    public async Task<ApiResponse<string>> RefreshTokenAsync(int userId)
    {
      try
      {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
          return ApiResponse<string>.ErrorResult("使用者不存在");
        }

        var newToken = GenerateJwtToken(user);
        return ApiResponse<string>.SuccessResult(newToken, "Token刷新成功");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "刷新Token時發生錯誤");
        return ApiResponse<string>.ErrorResult("Token刷新失敗");
      }
    }

    public async Task<ApiResponse<string>> UploadPhotoAsync(int userId, PhotoUploadRequest request)
    {
      try
      {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
          return ApiResponse<string>.ErrorResult("使用者不存在");
        }

        // 處理 Base64 字符串
        var base64Data = request.PhotoBase64;
        if (base64Data.Contains("data:image"))
        {
          // 如果包含前綴，則去除前綴部分
          // 例如 "data:image/png;base64, iVBORw0KGgoAAAANSUhEUgAAAAUA..."
          // 只保留實際的 Base64 編碼部分
          base64Data = base64Data.Split(',')[1];
        }
        // 建立上傳目錄
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userPhotos");
        if (!Directory.Exists(uploadDir))
        {
          Directory.CreateDirectory(uploadDir);
        }

        // 儲存檔案
        var fileName = $"user_{userId}_{request.FileName}";
        // 兩個string結合後的形式為: // wwwroot/userPhotos/user_1_photo.png，中間是使用斜線
        var filePath = Path.Combine(uploadDir, fileName);

        // 將 Base64 字符串轉換為位元組陣列並寫入檔案
        // Convert.FromBase64String 會將 Base64 字符串轉換為位元組陣列
        var bytes = Convert.FromBase64String(base64Data);
        await File.WriteAllBytesAsync(filePath, bytes);

        // 更新使用者資料
        user.PhotoPath = filePath;
        // SaveChangesAsync 會將變更儲存到資料庫
        await _context.SaveChangesAsync();

        return ApiResponse<string>.SuccessResult(filePath, "照片上傳成功");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "上傳照片時發生錯誤");
        return ApiResponse<string>.ErrorResult("照片上傳失敗");
      }
    }

    private static string HashPassword(string password)
    {
      // 使用 SHA256 哈希演算法來雜湊密碼
      // 這裡使用了配置中的 Salt，如果沒有配置則使用 "default_salt"
      // 注意：在實際應用中，建議使用更安全的哈希演算法，如 BCrypt
      var bcryptPassword = BCrypt.Net.BCrypt.HashPassword(password);
      return bcryptPassword;
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
      // 使用 BCrypt 驗證密碼
      // BCrypt.Net.BCrypt.Verify 方法會將輸入的密碼與儲存的雜湊密碼進行比對
      // 如果密碼正確，則返回 true，否則返回 false
      return HashPassword(password) == hashedPassword;
    }

    // 生成 JWT Token
    // 這個方法會根據使用者資訊生成一個 JWT Token
    private string GenerateJwtToken(User user)
    {
      var jwtSettings = _configuration.GetSection("JwtSettings");
      var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

      var token = new JwtSecurityToken(
          issuer: jwtSettings["Issuer"],
          audience: jwtSettings["Audience"],
          claims: claims,
          expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationMinutes"] ?? "60")),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
      var randomBytes = new byte[32];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomBytes);
      return Convert.ToBase64String(randomBytes);
    }
  }
}
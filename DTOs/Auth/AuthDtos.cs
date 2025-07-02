using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Auth
{
  // 這邊是註冊的DTO
  public class RegisterRequest
  {
    [Required(ErrorMessage = "用戶名稱不能空白。")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "用戶名稱長度必須在3到80個字元之間。")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼不能空白。")]
    [StringLength(128, MinimumLength = 6, ErrorMessage = "密碼長度必須在6到128個字元之間。")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "確認密碼不能空白。")]
    [Compare("Password", ErrorMessage = "密碼和確認密碼不一致。")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email不能空白。")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
  }

  // 這邊是登入的DTO
  public class LoginRequest
  {
    [Required(ErrorMessage = "用戶名稱不能空白。")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼不能空白。")]
    public string Password { get; set; } = string.Empty;
  }

  // 這邊是回傳的DTO
  public class AuthResponse
  {
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    // 這邊是使用者資訊，會回傳給前端，資料為UserDto
    public UserDto User { get; set; } = new();
  }

  // 定義使用者資訊 UserDto
  public class UserDto
  {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // ?表示這個屬性可以是null
    public string? PhotoPath { get; set; } = null;
    public string? SavePath { get; set; } = null;
    public DateTime CreatedAt { get; set; }
  }

  // api response
  // T 是泛型，可以是任何類型
  public class ApiResponse<T>
  {
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    // ?表示這個屬性可以是null
    // 假如T是string，則Data可以是string?，也就是可以是null或string
    // 假如T是UserDto，則Data可以是UserDto?，也就是可以是null或UserDto
    // 這樣的設計可以讓ApiResponse更通用，可以用於不同
    public T? Data { get; set; } = default;
    // new() 表示初始化一個空的 List<string>
    // [] 表示這是一個可為空的 List<string>
    // List指的是 System.Collections.Generic.List<T>，會在使用時指定具體的類型
    // 這邊使用List<string>，所以Errors可以存放多個錯誤訊息
    // 儲存型式範例: // Errors = new List<string> { "錯誤1", "錯誤2" }
    public List<string>? Errors { get; set; } = [];

    // static指的是這個方法是靜態的，可以直接透過類別名稱呼叫，而不需要實例化物件
    public static ApiResponse<T> SuccessResult(T data, string message = "操作成功")
    {
      return new ApiResponse<T>
      {
        Success = true,
        Message = message,
        Data = data
      };
    }
    
    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
      return new ApiResponse<T>
      {
        Success = false,
        Message = message,
        // ??指的是如果errors為null，則使用new List<string>()初始化一個空的List
        // 這樣可以避免在使用時出現NullReferenceException
        // new List<string>()無法使用，使用[]來初始化一個空的List
        Errors = errors ?? []
      };
    }
  }
}
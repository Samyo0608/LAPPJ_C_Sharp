using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
  public class User
  {
    public int Id { get; set; }

    // 使用者名稱
    [Required]
    [StringLength(80)]
    public string Username { get; set; } = string.Empty;

    // 使用者密碼
    [Required]
    [StringLength(128)]
    public string PasswordHash { get; set; } = string.Empty;

    // Email
    [Required]
    [StringLength(128)]
    public string Email { get; set; } = string.Empty;

    // 照片路徑
    [StringLength(256)]
    public string? PhotoPath { get; set; } = null;

    // 儲存路徑
    [StringLength(256)]
    public string? SavePath { get; set; } = null;

    // 建立時間
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // HashPassword
    public string HashPassword(string password)
    {
      // 使用BCrypt
      return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // 驗證密碼，輸入的密碼與儲存的密碼雜湊進行比對
    public bool VerifyPassword(string password)
    {
      // 使用BCrypt驗證密碼
      return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    // 設定密碼
    public void SetPassword(string password)
    {
      PasswordHash = HashPassword(password);
    }
  }
}
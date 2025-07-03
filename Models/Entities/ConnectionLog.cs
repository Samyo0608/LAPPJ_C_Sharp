using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
  public class ConnectionLog
  {
    public int Id { get; set; }

    // 設備種類
    [Required]
    [StringLength(50)]
    public string DeviceType { get; set; } = "";

    // Port
    [StringLength(20)]
    public string Port { get; set; } = "";

    // Address
    [StringLength(20)]
    public string Address { get; set; } = "";

    // IP 地址
    [StringLength(45)]
    public string IpAddress { get; set; } = "";

    // connect or disconnect
    [Required]
    [StringLength(10)]
    public string? Status { get; set; } = "";

    public bool Success { get; set; } = false;

    // 建立時間
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
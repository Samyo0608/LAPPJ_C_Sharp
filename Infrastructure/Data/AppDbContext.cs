using Microsoft.EntityFrameworkCore;
using backend.Models.Entities;

namespace backend.Infrastructure.Data
{
  public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
  {
    public DbSet<User> Users { get; set; }
    public DbSet<MeasurementData> MeasurementDatas { get; set; }
    public DbSet<ConnectionLog> ConnectionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>(entity =>
      {
        {
          // 主鍵
          entity.HasKey(u => u.Id);

          // unique索引，使用者名稱必須唯一
          entity.HasIndex(u => u.Username).IsUnique().HasDatabaseName("IX_Users_Username");
          // unique索引，Email必須唯一
          entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");
          // 設定資料表名稱
          entity.ToTable("Users");
          // 設定欄位屬性
          entity.Property(u => u.Username).IsRequired().HasMaxLength(80);
          entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(128);
          entity.Property(u => u.Email).IsRequired().HasMaxLength(128);
          entity.Property(u => u.PhotoPath).HasMaxLength(256);
          entity.Property(u => u.SavePath).HasMaxLength(256);
          entity.Property(u => u.CreatedAt).HasDefaultValueSql("datetime('now')");
        }
      });

      modelBuilder.Entity<MeasurementData>(entity =>
      {
        // 主鍵
        entity.HasKey(m => m.Id);
        // 設定資料表名稱
        entity.ToTable("MeasurementDatas");
        // 設定欄位屬性
        entity.Property(m => m.CreatedAt).HasDefaultValueSql("datetime('now')");
        entity.Property(m => m.MainGasFlowUnit).HasMaxLength(10);
        entity.Property(m => m.CarrierGasFlowUnit).HasMaxLength(10);
        entity.Property(m => m.CarrierGasType).HasMaxLength(20);
      });

      modelBuilder.Entity<ConnectionLog>(entity =>
      {
        // 主鍵
        entity.HasKey(c => c.Id);
        // 設定資料表名稱
        entity.ToTable("ConnectionLogs");
        // 設定欄位屬性
        entity.Property(c => c.DeviceType).IsRequired().HasMaxLength(50);
        entity.Property(c => c.Port).HasMaxLength(20);
        entity.Property(c => c.Address).HasMaxLength(20);
        entity.Property(c => c.IpAddress).HasMaxLength(45);
        entity.Property(c => c.Status).IsRequired().HasMaxLength(10);
        entity.Property(c => c.CreatedAt).HasDefaultValueSql("datetime('now')");
      });
    }
  }
}
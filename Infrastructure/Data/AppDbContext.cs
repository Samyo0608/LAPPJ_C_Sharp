using Microsoft.EntityFrameworkCore;
using backend.Models.Entities;

namespace backend.Infrastructure.Data
{
  public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
  {
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>(entity => {
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
          entity.Property(u => u.CreatedAt).HasDefaultValueSql("datatime('now')");
        }
      });
    }
  }
}
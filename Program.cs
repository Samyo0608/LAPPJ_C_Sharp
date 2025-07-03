using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Infrastructure.Data;
using backend.Services.Auth;
using backend.Services.Devices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 這個是有HTML的
// builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// 資料庫設定 - 使用SQLite
// 這邊的意思是使用SQLite資料庫，並且從appsettings.json的DefaultConnection取得連線字串
// 這個連線字串會在appsettings.json中定義
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊Service
// 這邊可以註冊其他的服務，例如使用者服務、認證服務等
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlicatService, AlicatService>();

// JWT驗證設定
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// 假如沒有設定SecretKey，則拋出異常
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
}

// 這邊要建立JWT驗證的服務
// 這個服務會使用JWT Bearer驗證方式
// JwtBearerDefaults.AuthenticationScheme是預設的驗證方案
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // 這邊設定JWT的驗證參數
    // TokenValidationParameters是用來設定JWT的驗證參數
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // ValidateIssuer和ValidAudience是用來驗證JWT的發行者和受眾
        ValidIssuer = jwtSettings["Issuer"] ?? "LAPPJ.API",
        ValidAudience = jwtSettings["Audience"] ?? "LAPPPJ.Client",
        // IssuerSigningKey是用來驗證JWT的簽名
        // 這邊使用對稱金鑰，使用UTF8編碼將SecretKey轉換為位元組陣列
        // SymmetricSecurityKey是用來建立對稱金鑰的
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// 註冊API服務
// 註冊Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LAPPJ backend", Version = "v1.0.0", Description = "LAPPJ C# API backend" });
    // 設定Swagger的JWT Bearer驗證
    c.AddSecurityDefinition("Bearer", new()
    {
        // 設定JWT Bearer的安全定義
        // Description是用來描述這個安全定義的
        Description = "請在這裡輸入您的JWT Token，格式為 Bearer {token}",
        // In是用來指定這個安全定義的位置
        // 這邊設定在Header中
        // Header是指HTTP請求的標頭
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        // Name是用來指定這個安全定義的名稱
        // 這邊設定為Authorization
        Name = "Authorization",
        // Type是用來指定這個安全定義的類型
        // 這邊設定為ApiKey，表示這是一個API金鑰
        // Scheme是用來指定這個安全定義的方案
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    { 
        // 這邊設定JWT Bearer的安全要求
        // 這個要求會在Swagger UI中顯示
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS設定
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    }
);

var app = builder.Build();


// swagger的路徑設定
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LAPPJ backend v1.0.0");
        c.RoutePrefix = "swagger"; // Swagger UI的路徑
    }
    );
}
else
{ 
    // 在非開發環境中，啟用HTTPS重定向
    // 這樣可以確保所有的HTTP請求都會被重定向
    app.UseHttpsRedirection();
}

app.UseCors("AllowAllOrigins");
// 先驗證再授權
// 這邊的順序很重要，必須先驗證再授權
app.UseAuthentication(); // 啟用JWT驗證
app.UseAuthorization(); // 啟用授權

// Map controllers
app.MapControllers();

// route設定
app.MapGet("/", () => new
{
    message = "LAPPJ API server is running",
    version = "1.0.0",
    swagger = "/swagger",
    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
});

// 建立資料庫
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // 確保資料庫已建立
    dbContext.Database.EnsureCreated();
}

app.Run();

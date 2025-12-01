using Microsoft.EntityFrameworkCore;
using BackEndFinal.Model;
using BackEndFinal.BUS;
using BackEndFinal.DAO;
using Microsoft.EntityFrameworkCore.Design;

var builder = WebApplication.CreateBuilder(args);


// Lấy port Render cấp
var connectionString =
    Environment.GetEnvironmentVariable("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// DI: Đăng ký các dịch vụ (DAO, BUS)
builder.Services.AddScoped<SinhVienDao>();
builder.Services.AddScoped<QuanLyHocTapBUS>();
builder.Services.AddScoped<UserDao>();

// Controllers & JSON Options (Tránh lỗi vòng lặp)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Swagger: Bật cho cả Production để dễ test
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// =========================================================
// 4. Xây dựng ứng dụng và cấu hình Pipeline
// =========================================================
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>(); // Thay AppDbContext bằng tên DbContext của bạn
        context.Database.Migrate(); // Lệnh này sẽ áp dụng các migration chưa chạy
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi khi cập nhật cơ sở dữ liệu.");
        // Bạn có thể quyết định dừng ứng dụng nếu lỗi migration nghiêm trọng
        // throw; 
    }
}
// HTTPS Redirection: Chỉ bật ở Development (tránh lỗi trên Render)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger UI: Bật cho cả Production
app.UseSwagger();
app.UseSwaggerUI();

// Kích hoạt CORS (Phải đặt trước Authorization)
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
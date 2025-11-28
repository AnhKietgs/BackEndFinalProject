using Microsoft.EntityFrameworkCore;
using BackEndFinal.Model;
using BackEndFinal.BUS;
using BackEndFinal.DAO;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// 1. QUAN TRỌNG: Cấu hình để lắng nghe trên PORT của Render
// =========================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080"; // Mặc định 8080 nếu không có biến PORT
builder.WebHost.UseUrls($"http://*:{port}"); // Lắng nghe trên tất cả IP với port đó

// =========================================================
// 2. Xử lý Chuỗi kết nối Database
// =========================================================
// Lấy connection string từ cấu hình (tự động ưu tiên biến môi trường)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Nếu là chuỗi từ Render (bắt đầu bằng postgresql://), cần sửa lại một chút
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgresql://"))
{
    connectionString = connectionString.Replace("postgresql://", "postgres://");
}

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


// =========================================================
// 3. Các cấu hình khác (CORS, DI, JSON, Swagger) - Giữ nguyên
// =========================================================

// CORS: Cho phép tất cả (Cân nhắc giới hạn lại khi chạy thật)
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

// HTTPS Redirection: Chỉ bật ở Development (tránh lỗi trên Render)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger UI: Bật cho cả Production
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackEndFinal API V1");
    c.RoutePrefix = string.Empty; // Để Swagger làm trang chủ (tùy chọn)
});

// Kích hoạt CORS (Phải đặt trước Authorization)
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
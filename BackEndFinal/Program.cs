using Microsoft.EntityFrameworkCore;
using BackEndFinal.Model;
using BackEndFinal.BUS;
using BackEndFinal.DAO;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080"; // Lấy PORT của Render, nếu không có thì mặc định 8080
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
// Thêm dịch vụ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()  // Cho phép mọi nguồn (máy tính của bạn) truy cập
                   .AllowAnyMethod()  // Cho phép GET, POST, PUT, DELETE
                   .AllowAnyHeader(); // Cho phép mọi header
        });
});
var app = builder.Build();

// ... các middleware khác ...

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();


builder.Services.AddScoped<SinhVienDao>();
builder.Services.AddScoped<QuanLyHocTapBUS>();
builder.Services.AddScoped<UserDao>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using BackEndFinal.Model;
using BackEndFinal.BUS;
using BackEndFinal.DAO;

var builder = WebApplication.CreateBuilder(args);

// Lấy port Render cấp
var connectionString =
    Environment.GetEnvironmentVariable("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

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

// DI
builder.Services.AddScoped<SinhVienDao>();
builder.Services.AddScoped<QuanLyHocTapBUS>();
builder.Services.AddScoped<UserDao>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Swagger (bật cả Production cho dễ test)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// KHÔNG bật HTTPS redirect trên Render
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger UI (cho cả Production)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

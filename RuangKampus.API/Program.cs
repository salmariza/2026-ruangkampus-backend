using Microsoft.EntityFrameworkCore;
using RuangKampus.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tambahkan CORS untuk mengizinkan akses dari frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()  // Mengizinkan akses dari semua origin (frontend kita)
              .AllowAnyMethod()  // Mengizinkan semua HTTP methods (GET, POST, PUT, DELETE)
              .AllowAnyHeader()); // Mengizinkan semua header
});

// Configure DbContext untuk SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Gunakan CORS untuk semua request
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

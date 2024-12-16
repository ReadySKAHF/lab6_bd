using Microsoft.EntityFrameworkCore;
using App.Middleware;
using App.Data;
using App;
using App.Service;
using Swashbuckle.AspNetCore.SwaggerGen; // Make sure to include this namespace
using System.Text.Json.Serialization; // Ensure you have this one as well

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

// Register Swagger services
builder.Services.AddSwaggerGen(); // Add this line to register Swagger services

// Register caching
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CachedService>();

// Register session
builder.Services.AddSession();

// Register DbContext with connection string
builder.Services.AddDbContext<AutoRepairWorkshopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // Включаем аннотации Swagger
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger generation
    app.UseSwaggerUI(); // Enable Swagger UI
}

app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

// Enable custom DB initialization middleware
app.UseDbInitializer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=cars}/{action=Index}/{id?}");

app.Run();

using CmsBackend.Application.Auth.Interfaces;
using CmsBackend.Application.Auth.Services;
using CmsBackend.Application.Blogs.Interfaces;
using CmsBackend.Application.Blogs.Services;
using CmsBackend.Application.Users.Interfaces;
using CmsBackend.Application.Users.Services;
using CmsBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// DB CONFIG (PostgreSQL)
// ==========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================
// SERVICES
// ========================== 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBlogService, BlogService>();

// ==========================
// OPEN API + SCALAR
// ==========================
builder.Services.AddOpenApi();




//============================
//Setup Env
//============================

bool isDev = builder.Environment.IsDevelopment();

//=============================
// Add CORS
//=============================

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        if (isDev)
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins("https://your-frontend.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});


// ==========================
// AUTH (JWT)
// ==========================
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// ==========================
// CONTROLLERS
// ==========================
//This will convert enum numbers 1, 2, 3 into "Publish", "Draft"
builder.Services.AddControllers().AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters
             .Add(new JsonStringEnumConverter());
     });

var app = builder.Build();

// ==========================
// OPEN API ENDPOINT
// ==========================
app.MapOpenApi();

// ==========================
// SCALAR UI (ENABLE IN PROD)
// ==========================
app.MapScalarApiReference(options =>
{
    options.WithTitle("CMS Blog API");
});

// ==========================
// MIDDLEWARE
// ==========================
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================
// RAILWAY PORT FIX (IMPORTANT)
// ==========================

if (!isDev)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://0.0.0.0:{port}");
}


// ==========================
app.Run();
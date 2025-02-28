using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services;
using Infrastructure.IServices;
using CoinGecko.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.StackExchangeRedis; // <-- For Redis

var builder = WebApplication.CreateBuilder(args);

// 1) CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policyBuilder => policyBuilder
            .WithOrigins("http://localhost:4200") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// 2) Database
builder.Services.AddDbContext<ApplicationContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 3) Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

// 4) Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    // Replace with your actual Redis endpoint. For example:
    // "localhost:6379" or "your-redis-host:6379"  docker example
    options.Configuration = "localhost:6379";
    options.InstanceName = "CoinHawk:"; 
});

// 5) CoinGecko + Services
builder.Services.AddSingleton<CoinGeckoClient>();

// Inject the Distributed Cache into CoinService
builder.Services.AddScoped<ICoinService, CoinService>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<CoinGeckoClient>();
    var cache = serviceProvider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
    return new CoinService(client, cache);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPriceAlertService, PriceAlertService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddHttpClient<INewsService, NewsService>();

// 6) Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(PriceAlert), 200));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.MaxDepth = 32;
});

// 7) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 8) JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

var app = builder.Build();

// 9) Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

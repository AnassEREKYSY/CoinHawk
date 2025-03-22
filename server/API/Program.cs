using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
using Microsoft.Extensions.Caching.StackExchangeRedis;

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
    // For local Docker Redis
    options.Configuration = "redis:6379";
    options.InstanceName = "CoinHawk:";
});

// 5) External services & custom services

// 5a) CoinGecko Client
builder.Services.AddSingleton<CoinGeckoClient>();

// 5b) Services
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
builder.Services.AddSingleton<IJwtTokenDecoderService, JwtTokenDecoderService>();

// 6) Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(PriceAlert), 200));
})
.AddJsonOptions(options =>
{
    // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// 7) Swagger (for development / API exploration)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
 * 8) Keycloak via OpenID Connect
 * 
 * This configuration sets:
 *   - The default scheme to Cookies (for maintaining the local session)
 *   - The challenge scheme to OpenID Connect (triggering Keycloak login)
 *   - The callback route is set to "/api/users/login" so that once Keycloak authenticates
 *     the user, it redirects back to this endpoint (which is handled automatically by the OIDC middleware).
 */
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8082/realms/CoinHawkRealm";
        options.RequireHttpsMetadata = false; // HTTP allowed in dev
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = new[] { "coin-hawk-app", "account" },
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8082/realms/CoinHawkRealm",
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        // Important for HTTP (Dev Only)
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });

var app = builder.Build();

// 9) Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("browser/index.html");

app.Run();


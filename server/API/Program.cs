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
    options.Configuration = "localhost:6379";
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
 * 8) (Commented Out) Old JWT Authentication
 * Uncomment if you still want to validate locally-issued JWTs.
 *
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
*/

// 8) Keycloak via OpenID Connect
builder.Services
    .AddAuthentication(options =>
    {
        // The default scheme for user sign-in
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // The default challenge scheme is OIDC
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie() // We need cookies to track the user's logged-in session
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "http://localhost:8082/realms/myrealm";

        options.ClientId = "coin-hawk-app";

        // For a “confidential” client, specify your client secret
        options.ClientSecret = "<Keycloak Client Secret>";

        // Use the standard code flow
        options.ResponseType = "code";

        // Must match the redirect URI set in Keycloak
        // E.g. http://localhost:5000/signin-oidc or similar
        options.CallbackPath = "/signin-oidc";

        // Save tokens in the auth session if you need them later
        options.SaveTokens = true;

        // If Keycloak is running on HTTP in dev, you might disable HTTPS metadata
        // options.RequireHttpsMetadata = false;
    });

var app = builder.Build();

// 9) Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Enable auth (cookies + OIDC)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

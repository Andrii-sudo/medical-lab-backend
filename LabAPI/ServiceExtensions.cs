using LabAPI.Models;
using LabAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LabAPI;
public static class CorsPolicies
{
    public const string Angular = "Angular";
}

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<MedicalLabsContext>(options =>
        {
            options.UseSqlServer(config["ConnectionStrings:DefaultConnection"]);
        });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        string jwtKey = config["Jwt:Key"] ?? throw new Exception("JWT Key is missing in secrets");
     
        services.AddIdentity<AppUser, IdentityRole<int>>(options =>
        {
            options.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<MedicalLabsContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
            // Щоб читати з cookie
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["jwt_token"];
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.Angular, policy =>
            {
                policy.WithOrigins(config["Cors:Origin"]!)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}


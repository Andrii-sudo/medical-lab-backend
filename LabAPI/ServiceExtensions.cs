using LabAPI.Models;
using LabAPI.Services;
using LabAPI.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;

namespace LabAPI;
public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<MedicalLabsContext>(options =>
        {
            options.UseSqlServer(config["ConnectionStrings:DefaultConnection"]);
        });

        services.AddHangfire(cfg => 
            cfg.UseSqlServerStorage(config["ConnectionStrings:HangfireConnection"]));
        services.AddHangfireServer();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISampleService, SampleService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAnalysesService, AnalysesService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        string jwtKey = config["Jwt:Key"] ?? throw new Exception("JWT Key is missing in secrets");
     
        services.AddIdentity<AppUser, IdentityRole<int>>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false; 
            options.Password.RequireLowercase = false;       
            options.Password.RequireUppercase = false;       
            options.Password.RequireDigit = false;
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


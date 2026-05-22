using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Data;
using Shared.Security;

namespace Shared.Extensions;

/// <summary>
/// Extensiones reutilizables para registrar servicios comunes en cada microservicio.
/// Aplica DRY: la configuración de JWT, EF Core y CORS se define una sola vez.
/// </summary>
public static class ServiceCollectionExtensions
{
    public const string CorsPolicy = "FrontendPolicy";

    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        // EF Core con SQL Server. EnableRetryOnFailure: reintenta ante fallos
        // transitorios (útil mientras el contenedor de SQL Server termina de arrancar).
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        // Configuración JWT desde appsettings
        var jwtSection = config.GetSection("Jwt");
        services.Configure<JwtSettings>(jwtSection);
        var jwtSettings = jwtSection.Get<JwtSettings>()!;
        services.AddSingleton<IJwtService, JwtService>();

        // Autenticación JWT Bearer
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // CORS: solo el frontend Angular puede consumir las APIs
        var frontendUrl = config["FrontendUrl"] ?? "http://localhost:4400";
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
                policy.WithOrigins(frontendUrl)
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });

        return services;
    }
}

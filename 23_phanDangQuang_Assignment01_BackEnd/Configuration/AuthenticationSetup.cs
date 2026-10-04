using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using _23_phanDangQuang_Assignment01_BackEnd.DataAccess.Daos;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories;
using _23_phanDangQuang_Assignment01_BackEnd.Repositories.Interfaces;
using _23_phanDangQuang_Assignment01_BackEnd.Services;
using _23_phanDangQuang_Assignment01_BackEnd.Services.Interfaces;

namespace _23_phanDangQuang_Assignment01_BackEnd.Configuration;

public static class AuthenticationSetup
{
    public static IServiceCollection AddBackendAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AdminAccountOptions>().Bind(configuration.GetSection("AdminAccount"))
            .ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations().ValidateOnStart();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AccountAuthDao>();
        services.AddScoped<IAccountAuthRepository, AccountAuthRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<JwtTokenIssuer>();
        services.AddScoped<AccountTokenValidator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, configured) =>
            {
                var settings = configured.Value;
                bearer.MapInboundClaims = false;
                bearer.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => context.HttpContext.RequestServices
                        .GetRequiredService<AccountTokenValidator>().ValidateAsync(context)
                };
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, ValidIssuer = settings.Issuer,
                    ValidateAudience = true, ValidAudience = settings.Audience,
                    ValidateLifetime = true, RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true, RequireSignedTokens = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = JwtRegisteredClaimNames.Name, RoleClaimType = "role"
                };
            });
        services.AddAuthorization();
        return services;
    }
}

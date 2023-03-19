using System;
using System.Text;
using ChessVariantsAPI.Authentication;
using ChessVariantsAPI.GameOrganization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace ChessVariantsAPI;

/// <summary>
/// Class where all extension methods to <see cref="IServiceCollection"/> should live.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <see cref="GameOrganizer"/> as a singleton to the dependency injection system.
    /// </summary>
    /// <param name="services">The service collection to add the singleton to.</param>
    /// <returns>The input <paramref name="services"/> in order to enable method chaining.</returns>
    public static IServiceCollection AddGameOrganzation(this IServiceCollection services)
    {
        services.AddSingleton<GameOrganizer>();
        return services;
    }

    public static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var key = configuration["Authentication:JWTSecret"];
        services.AddTransient(jwt => new JWTUtils(key));
        var byteKey = Encoding.ASCII.GetBytes(key);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(byteKey),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/game")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        var serviceCollection = services.AddSingleton<IUserIdProvider, UsernameBasedUserIdProvider>();

        return services;
    }
}


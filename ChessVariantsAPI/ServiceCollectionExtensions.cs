using System;
using System.Text;
using ChessVariantsAPI.Authentication;
using ChessVariantsAPI.GameOrganization;
using ChessVariantsAPI.Hubs;
using DataAccess.MongoDB;
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
    /// Adds a <see cref="GameOrganizer"/> and a <see cref="GroupOrganizer"/> as singletons to the dependency injection system.
    /// </summary>
    /// <param name="services">The service collection to add the singleton to.</param>
    /// <returns>The input <paramref name="services"/> in order to enable method chaining.</returns>
    public static IServiceCollection AddOrganization(this IServiceCollection services)
    {
        services.AddSingleton<GameOrganizer>();
        services.AddSingleton<GroupOrganizer>();
        return services;
    }

    public static IServiceCollection AddEditorOrganization(this IServiceCollection services)
    {
        services.AddSingleton<EditorOrganizer>();
        return services;
    }

    /// <summary>
    /// Adds authentication to the program and configures JWT both for http and SignalR.
    /// </summary>
    /// <param name="services">the services</param>
    /// <param name="configuration">the configuration, which has to include the JWT secret</param>
    /// <returns>services for method chaining</returns>
    public static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var configKey = "Authentication:JWTSecret";
        var secretKey = configuration[configKey];
        if (secretKey == null)
        {
            throw new ConfigurationValueNotFoundException($"No value found for the secretKey. Make sure that the user-secret for '{configKey}' is set.");
        }
        services.AddTransient(jwt => new JWTUtils(secretKey));
        var secretByteKey = Encoding.ASCII.GetBytes(secretKey);

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
                IssuerSigningKey = new SymmetricSecurityKey(secretByteKey),
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

    public static IServiceCollection AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var configKey = "MongoDatabase:ConnectionString";
        var connectionString = configuration[configKey];
        if (connectionString == null)
        {
            throw new ConfigurationValueNotFoundException($"No value found for the secretKey. Make sure that the user-secret for '{configKey}' is set.");
        }
        services.AddSingleton<DatabaseService>(new TestDatabaseService(connectionString));
        return services;
    }
}

public class ConfigurationValueNotFoundException : Exception
{
    public ConfigurationValueNotFoundException(string message) : base(message) { }
}


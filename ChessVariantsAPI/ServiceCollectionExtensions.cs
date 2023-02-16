using System;
using ChessVariantsAPI.GameOrganization;

namespace ChessVariantsAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameOrganzation(this IServiceCollection services)
    {
        services.AddSingleton<GameOrganizer>();
        return services;
    }
}


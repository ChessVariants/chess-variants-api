using System;
using ChessVariantsAPI.GameOrganization;

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
    public static IServiceCollection AddEditorOrganization(this IServiceCollection services)
    {
        services.AddSingleton<EditorOrganizer>();
        return services;
    }

}


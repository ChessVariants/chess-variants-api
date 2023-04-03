using ChessVariantsAPI.Hubs;

namespace ChessVariantsAPI;

/// <summary>
/// Class where all extension methods to <see cref="WebApplication"/> should live.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Maps all SignalR hubs to a path.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> object to map the hubs to.</param>
    /// <returns>returns the input <paramref name="app"/> in order to enable method chaining.</returns>
    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<GameHub>("/game");
        app.MapHub<EditorHub>("/editor");
        return app;
    }
}

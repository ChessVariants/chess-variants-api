using ChessVariantsAPI.Hubs;

namespace ChessVariantsAPI;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<GameHub>("/game");
        return app;
    }
}

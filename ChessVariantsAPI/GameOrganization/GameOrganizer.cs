using System;
namespace ChessVariantsAPI.GameOrganization;

public class GameOrganizer
{
    private readonly Dictionary<string, string> _activeGames;

    public GameOrganizer()
    {
        _activeGames = new Dictionary<string, string>();
    }

    public void CreateNewGame(string gameId)
    {
        if (_activeGames.ContainsKey(gameId))
        {
            return;
        }
        _activeGames.Add(gameId, "placeholder game");
    }

    public string GetGame(string gameId)
    {
        var game = _activeGames.GetValueOrDefault(gameId, "");
        if (game == "")
        {
            throw new GameNotFoundException($"No active game for gameId: {gameId}");
        }
        return game;
     }

    private void DeleteGame(string gameId)
    {
        _activeGames.Remove(gameId);
    }
}

public class GameNotFoundException : Exception
{
    public GameNotFoundException(string message) : base(message) {}
}


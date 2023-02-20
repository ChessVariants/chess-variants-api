namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// This class is responsible for organizing active games by mapping a game ID to a corresponding game object (currently string as placeholder).
/// </summary>
public class GameOrganizer
{
    private readonly Dictionary<string, string> _activeGames;

    public GameOrganizer()
    {
        _activeGames = new Dictionary<string, string>();
    }

    /// <summary>
    /// Creates a new game and maps it to the given gameId. If the gameId already maps to a game, nothing will happen.
    /// </summary>
    /// <param name="gameId">The key to which the created game should be mapped to.</param>
    public void CreateNewGame(string gameId)
    {
        if (_activeGames.ContainsKey(gameId))
        {
            return;
        }
        _activeGames.Add(gameId, "placeholder game");
    }

    /// <summary>
    /// Returns the game object for the given gameId if it exists, otherwise throws a <c>GameNotFoundException</c>
    /// </summary>
    /// <param name="gameId">The game id for the game to return</param>
    /// <returns>The game object corresponding to the gameId</returns>
    /// <exception cref="GameNotFoundException"></exception>
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


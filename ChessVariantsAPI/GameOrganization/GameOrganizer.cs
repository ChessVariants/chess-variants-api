using ChessVariantsLogic;

namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// This class is responsible for organizing active games by mapping a game ID to a corresponding game object (currently string as placeholder).
/// </summary>
public class GameOrganizer
{
    private readonly Dictionary<string, GameMapping?> _activeGames;
    private static readonly Dictionary<string, Player?> _colorDict = new Dictionary<string, Player?>(){
        {"white", Player.White},
        {"black", Player.Black}
    };

    public GameOrganizer()
    {
        _activeGames = new Dictionary<string, GameMapping?>();
    }

    /// <summary>
    /// Creates a new game and maps it to the given <paramref name="gameId"/>. If the <paramref name="gameId"/> already maps to a game, nothing will happen.
    /// </summary>
    /// <param name="gameId">The key to which the created game should be mapped to.</param>
    public bool JoinGame(string gameId, string playerIdentifier, string asColor)
    {
        var gameMapping = _activeGames.GetValueOrDefault(gameId, null);
        if (gameMapping == null) {
            gameMapping = new GameMapping(GameFactory.StandardChess()); // TODO don't automatically create standard chess
            _activeGames.Add(gameId, gameMapping); 
        }

        var color = _colorDict.GetValueOrDefault(asColor, null);
        if (color == null) {
            throw new InvalidColorException($"{asColor} is not a valid color. Only 'black' or 'white' are valid as colors.");
        }
        try 
        {
            gameMapping.addPlayer(playerIdentifier, (Player) color);
            return true;
        }
        catch (PlayerAlreadyExistsException) {
            return false;
        }
    }

    public void LeaveGame(string gameId, string playerIdentifier) {
        var gameMapping = _activeGames.GetValueOrDefault(gameId, null);
        if (gameMapping != null) {
            gameMapping.removePlayer(playerIdentifier);
        } 
    }

    /// <summary>
    /// Returns the game object for the given <paramref name="gameId"/> if it exists, otherwise throws a <see cref="GameNotFoundException"/>
    /// </summary>
    /// <param name="gameId">The game id for the game to return</param>
    /// <returns>The game object corresponding to the gameId</returns>
    /// <exception cref="GameNotFoundException"></exception>
    public Game GetGame(string gameId)
    {
        var gameMapping = _activeGames.GetValueOrDefault(gameId, null);
        if (gameMapping == null)
        {
            throw new GameNotFoundException($"No active game for gameId: {gameId}");
        }
        return gameMapping.GetGame();
    }

    private void DeleteGame(string gameId)
    {
        _activeGames.Remove(gameId);
    }
}

public class GameMapping {
    private readonly Game _game;
    private readonly Dictionary<string, Player?> _playerDict;

    public GameMapping(Game game)
    {
        _game = game;
        _playerDict = new Dictionary<string, Player?>();
    }

    public void addPlayer(string playerIdentifier, Player color) {
        if (_playerDict.Values.Contains(color)) {
            throw new PlayerAlreadyExistsException($"A player of color {color} already exists in this game");
        }
        _playerDict.Add(playerIdentifier, color);
    }

    public void removePlayer(string playerIdentifier) {
        _playerDict.Remove(playerIdentifier);
    }

    public Player? getPlayer(string playerIdentifier) {
        return _playerDict.GetValueOrDefault(playerIdentifier, null);
    }

    public Game GetGame() {
        return _game;
    }


}

/// <summary>
/// Exception for when a game is not found amongst the active games.
/// </summary>
public class GameNotFoundException : Exception
{
    public GameNotFoundException(string message) : base(message) {}
}

public class InvalidColorException : Exception {
    public InvalidColorException(string message) : base(message) {}
}

public class PlayerAlreadyExistsException : Exception {
    public PlayerAlreadyExistsException(string message) : base(message) {}
}


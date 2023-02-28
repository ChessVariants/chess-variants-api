using ChessVariantsLogic;

namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// This class is responsible for organizing active games by mapping a game ID to a corresponding game object.
/// </summary>
public class GameOrganizer
{
    private readonly Dictionary<string, ActiveGame?> _activeGames;
    private readonly static Dictionary<string, Player?> _colorDict = new()
    {
        { "white", Player.White },
        { "black", Player.Black }
    };

    public GameOrganizer()
    {
        _activeGames = new Dictionary<string, ActiveGame?>();
    }

    /// <summary>
    /// Creates a new game if one does not already exist with <paramref name="gameId"/> and adds the player to the game if possible.
    /// If <paramref name="asColor"/> is invalid an <see cref="InvalidColorException"/> will surface.
    /// </summary>
    /// <param name="gameId">The key to which the created game should be mapped to.</param>
    /// <returns>True if the player could join the game, false otherwise</returns>
    /// <exception cref="InvalidColorException">If the color was not found (=null)</exception>
    public bool JoinGame(string gameId, string playerIdentifier, string asColor)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        activeGame = CreateGameIfNull(gameId, activeGame);
        var color = _colorDict.GetValueOrDefault(asColor, null);

        try
        {
            activeGame.AddPlayer(playerIdentifier, color);
            return true;
        }
        catch (PlayerAlreadyExistsException)
        {
            return false;
        }
    }

    private ActiveGame CreateGameIfNull(string gameId, ActiveGame? activeGame)
    {
        if (activeGame == null)
        {
            activeGame = new ActiveGame(GameFactory.StandardChess()); // TODO don't automatically create standard chess
            _activeGames.Add(gameId, activeGame);
        }
        return activeGame;
    }

    /// <summary>
    /// Removes a player with identifier <paramref name="playerIdentifier"/> from the game with id <paramref name="gameId"/>.
    /// </summary>
    /// <param name="gameId">The gameId for the game to leave</param>
    /// <param name="playerIdentifier">The player identifier for the player to leave</param>
    /// <returns>True if the player successfully left the game, otherwise false.</returns>
    public bool LeaveGame(string gameId, string playerIdentifier)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        if (activeGame != null)
        {  
            // TODO remove game from _activeGames if it is empty if we want
            return activeGame.RemovePlayer(playerIdentifier);
        }
        return false;
    }

    /// <summary>
    /// Returns the game object for the given <paramref name="gameId"/> if it exists, otherwise throws a <see cref="GameNotFoundException"/>
    /// </summary>
    /// <param name="gameId">The game id for the game to return</param>
    /// <returns>The game object corresponding to the gameId</returns>
    /// <exception cref="GameNotFoundException"></exception>
    public Game GetGame(string gameId)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        if (activeGame == null)
        {
            throw new GameNotFoundException($"No active game for gameId: {gameId}");
        }
        return activeGame.GetGame();
    }

    /// <summary>
    /// Returns the player object for the given <paramref name="gameId"/> and <paramref name="playerIdentifier"/> if it exists, otherwise throws correct exceptions.
    /// </summary>
    /// <param name="gameId">The game id of the game the connected user plays</param>
    /// <param name="playerIdentifier">Connection id of the player to return</param>
    /// <returns>The player object corresponding to the gameId and playerIdentifier</returns>
    /// <exception cref="GameNotFoundException">If the game was not found (=null)</exception>
    /// <exception cref="PlayerNotFoundException">If the connectionId was not found</exception>
    public Player? GetPlayer(string gameId, string playerIdentifier)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        if (activeGame == null)
        {
            throw new GameNotFoundException($"No active game for gameId: {gameId}");
        }
        Player? player = activeGame.GetPlayer(playerIdentifier);
        if (player == null)
        {
            throw new PlayerNotFoundException($"No player with identifier {playerIdentifier} found in game {gameId}");
        }
        return player;
    }

    private void DeleteGame(string gameId)
    {
        _activeGames.Remove(gameId);
    }
}

/// <summary>
/// Represents an active game (not necessarily in started) which maps player identifiers to a <see cref="Player"/> type,
/// and the game they are mapped to.
/// </summary>
public class ActiveGame
{

    private readonly Game _game;
    private readonly Dictionary<string, Player?> _playerDict;

    public ActiveGame(Game game)
    {
        _game = game;
        _playerDict = new Dictionary<string, Player?>();
    }

    /// <summary>
    /// Maps the <paramref name="playerIdentifier"/> to a color as supplied.
    /// </summary>
    /// <param name="playerIdentifier">The player identifier</param>
    /// <param name="color">The color to map the player identifier to</param>
    /// <exception cref="InvalidColorException">If the color is null</exception>
    /// <exception cref="PlayerAlreadyExistsException">If a player is already mapped to <paramref name="color"/></exception>
    public void AddPlayer(string playerIdentifier, Player? color)
    {
        if (color == null)
        {
            throw new InvalidColorException($"{color} is not a valid color. Only 'black' or 'white' are valid as colors.");
        }
        if (_playerDict.ContainsValue(color))
        {
            throw new PlayerAlreadyExistsException($"A player of color {color} already exists in this game");
        }
        _playerDict.Add(playerIdentifier, color);
    }

    /// <summary>
    /// Removes a player from the active game, if they are in it.
    /// </summary>
    /// <param name="playerIdentifier">The identifier for the player to remove</param>
    /// <returns>True if the player was removed, otherwise false</returns>
    public bool RemovePlayer(string playerIdentifier)
    {
        return _playerDict.Remove(playerIdentifier);
    }

    /// <summary>
    /// Returns the <see cref="Player"/> enum mapped to the <paramref name="playerIdentifier"/>.
    /// If not found then returns null.
    /// </summary>
    /// <param name="playerIdentifier"></param>
    /// <returns>The player if found, otherwise null.</returns>
    public Player? GetPlayer(string playerIdentifier)
    {
        return _playerDict.GetValueOrDefault(playerIdentifier, null);
    }

    /// <summary>
    /// Returns the game
    /// </summary>
    /// <returns>returns the game</returns>
    public Game GetGame()
    {
        return _game;
    }
}

/// <summary>
/// Exception for when a game is not found amongst the active games.
/// </summary>
public class GameNotFoundException : Exception
{
    public GameNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception for when a supplied chess color is not one of the valid ones (black/white)
/// </summary>
public class InvalidColorException : Exception
{
    public InvalidColorException(string message) : base(message) { }
}

/// <summary>
/// Exception for when a player of a certain type already exists.
/// </summary>
public class PlayerAlreadyExistsException : Exception
{
    public PlayerAlreadyExistsException(string message) : base(message) { }
}

/// <summary>
/// Exception for when a player is not found in a game.
/// </summary>
public class PlayerNotFoundException : Exception
{
    public PlayerNotFoundException(string message) : base(message) { }
}

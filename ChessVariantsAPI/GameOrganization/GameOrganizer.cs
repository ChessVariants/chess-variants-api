using ChessVariantsLogic;

namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// This class is responsible for organizing active games by mapping a game ID to a corresponding game object.
/// </summary>
public class GameOrganizer
{
    private readonly Dictionary<string, ActiveGame?> _activeGames;

    public GameOrganizer()
    {
        _activeGames = new Dictionary<string, ActiveGame?>();
    }

    #region Administration

    /// <summary>
    /// Creates a new game if one does not already exist with <paramref name="gameId"/> and adds the player to the game if possible.
    /// </summary>
    /// <param name="gameId">The key to which the created game should be mapped to.</param>
    /// <returns>True if the player could join the game, false otherwise</returns>
    public Player JoinGame(string gameId, string playerIdentifier)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        if (activeGame == null)
        {
            throw new GameNotFoundException();
        }
        return activeGame.AddPlayer(playerIdentifier);
    }

    public Player CreateGame(string gameId, string playerIdentifier)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        if (activeGame != null)
        {
            throw new OrganizerException("The game you're trying to create already exists");
        }
        activeGame = CreateGameIfNull(gameId, activeGame, playerIdentifier);
        return (Player) activeGame.GetPlayer(playerIdentifier)!;
    }

    private ActiveGame CreateGameIfNull(string gameId, ActiveGame? activeGame, string playerIdentifier)
    {
        if (activeGame == null)
        {
            activeGame = new ActiveGame(GameFactory.StandardChess(), playerIdentifier); // TODO don't automatically create standard chess
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
            return activeGame.RemovePlayer(playerIdentifier);
        }
        return false;
    }

    /// <summary>
    /// Returns the game object for the given <paramref name="gameId"/> if it exists, otherwise surfaces a <see cref="GameNotFoundException"/>
    /// </summary>
    /// <param name="gameId">The game id for the game to return</param>
    /// <returns>The game object corresponding to the gameId</returns>
    /// <exception cref="GameNotFoundException"></exception>
    public Game GetGame(string gameId)
    {
        var activeGame = GetActiveGame(gameId);
        return activeGame.GetGame();
    }

    private ActiveGame GetActiveGame(string gameId)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        if (activeGame == null)
        {
            throw new GameNotFoundException($"No active game for gameId: {gameId}");
        }
        return activeGame;
    }

    /// <summary>
    /// Gets the player corresponding to the provided <paramref name="gameId"/> and <paramref name="playerIdentifier"/>.
    /// </summary>
    /// <param name="gameId">The game id for the to find the <see cref="Player"/> for</param>
    /// <param name="playerIdentifier">The unique identifier for the <see cref="Player"/> to find</param>
    /// <returns>The player if found</returns>
    /// <exception cref="PlayerNotFoundException">Is raised if there is no such player for the supplied gameId</exception>
    public Player GetPlayer(string gameId, string playerIdentifier)
    {
        var activeGame = GetActiveGame(gameId);
        var player = activeGame.GetPlayer(playerIdentifier);
        if (player == null)
        {
            throw new PlayerNotFoundException($"No player with identifier '{playerIdentifier}' found for game with gameId '{gameId}'");
        }
        return (Player) player;
    }

    /// <summary>
    /// Swaps the active players colors to the opposite. Only works if the requester is the admin.
    /// </summary>
    /// <param name="gameId">The game to swap colors in</param>
    /// <param name="playerIdentifier">The player requesting the swap</param>
    public void SwapColors(string gameId, string playerIdentifier)
    {
        var activeGame = GetActiveGame(gameId);
        activeGame.SwapColors(playerIdentifier);
    }

    /// <summary>
    /// Removes a game
    /// </summary>
    /// <param name="gameId">The game to remove</param>
    public void DeleteGame(string gameId)
    {
        _activeGames.Remove(gameId);
    }

    /// <summary>
    /// Gets the current state of the game as a json string
    /// </summary>
    /// <param name="gameId">The game to get the state for</param>
    /// <returns>A json-formatted string of current game state</returns>
    public string GetStateAsJson(string gameId)
    {
        var game = GetGame(gameId);
        return game.ExportStateAsJson();
    } 

    #endregion

    #region GameControl

    public GameEvent Move(string move, string gameId, string playerIdentifier)
    {
        var game = GetGame(gameId);
        var player = GetPlayer(gameId, playerIdentifier);
        GameEvent result = game.MakeMove(move, player);
        return result;
    }

    #endregion
}



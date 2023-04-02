using ChessVariantsAPI.Hubs;
using ChessVariantsLogic;
using ChessVariantsLogic.Export;

namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// This class is responsible for organizing active games by mapping a game ID to a corresponding game object.
/// </summary>
public class GameOrganizer
{
    private readonly Dictionary<string, ActiveGame?> _activeGames;
    private readonly ILogger _logger;

    public GameOrganizer(ILogger<GameOrganizer> logger)
    {
        _activeGames = new Dictionary<string, ActiveGame?>();
        _logger = logger;
    }

    public void GetGameIdsForPlayer(string playerIdentifier)
    {
        var gameIds = new List<string>();
        foreach (var game in _activeGames.Values)
        {
            var idPlayerPairs = game?.GetPlayers();
            if (idPlayerPairs == null) continue;
            foreach (var pair in idPlayerPairs)
            {
                if (pair.Item1 == playerIdentifier) gameIds.Add(pair.Item1);
            }
        }
    }

    public bool PlayerAbleToJoin(string gameId, string playerIdentifier)
    {
        var players = GetActiveGame(gameId).GetPlayers();
        return (PlayerInGame(gameId, playerIdentifier) || 2 - players.Count > 0);
    }

    public bool PlayerInGame(string gameId, string playerIdentifier)
    {
        var players = GetActiveGame(gameId).GetPlayers();
        var bools = players.Select(pair => pair.Item1 == playerIdentifier);
        return bools.Any(x => x == true);
    }

    #region Administration

    /// <summary>
    /// Creates a new game if one does not already exist with <paramref name="gameId"/> and adds the player to the game if possible.
    /// </summary>
    /// <param name="gameId">The key to which the created game should be mapped to.</param>
    /// <returns>True if the player could join the game, false otherwise</returns>
    public Player JoinGame(string gameId, string playerIdentifier)
    {
        var activeGame = GetActiveGame(gameId);
        return activeGame.AddPlayer(playerIdentifier);
    }

    public Player CreateGame(string gameId, string playerIdentifier, string variantIdentifier=GameFactory.StandardIdentifier)
    {
        AssertGameDoesNotExist(gameId);
        var activeGame = CreateActiveGame(gameId, playerIdentifier, variantIdentifier);
        return activeGame.GetPlayer(playerIdentifier)!;
    }

    public bool SetGame(string gameId, string playerIdentifier, string variantIdentifier=GameFactory.StandardIdentifier)
    {
        var activeGame = GetActiveGame(gameId);
        var gameVariant = CreateGameInstance(variantIdentifier);
        return activeGame.SetGame(gameVariant, playerIdentifier, variantIdentifier);
    }

    public bool StartGame(string gameId, string playerIdentifier)
    {
        return GetActiveGame(gameId).StartGame(playerIdentifier);
    }

    public Player CreateLobby(string gameId, string playerIdentifier)
    {
        AssertGameDoesNotExist(gameId);
        var activeGame = new ActiveGame(playerIdentifier);
        _activeGames.Add(gameId, activeGame);
        return activeGame.GetPlayer(playerIdentifier)!;
    }

    private void AssertGameDoesNotExist(string gameId)
    {
        var activeGame = _activeGames.GetValueOrDefault(gameId, null);
        if (activeGame != null)
        {
            throw new OrganizerException($"The game (id: {gameId}) you're trying to create already exists");
        }
    }

    private ActiveGame CreateActiveGame(string gameId, string playerIdentifier, string variantIdentifier)
    {
        var gameVariant = CreateGameInstance(variantIdentifier);
        var activeGame = new ActiveGame(gameVariant, playerIdentifier, variantIdentifier);
        _activeGames.Add(gameId, activeGame);
        return activeGame;
    }

    private static Game CreateGameInstance(string variantIdentifier)
    {
        try
        {
            return GameFactory.FromIdentifier(variantIdentifier);
        }
        catch (ArgumentException e)
        {
            throw new OrganizerException(e.Message);
        }
    }

    /// <summary>
    /// Returns what chess variant the game is
    /// </summary>
    /// <param name="gameId">The id for the game to query what variant it is</param>
    /// <returns>The chess variant for the supplied <paramref name="gameId"/></returns>
    public string GetActiveGameVariant(string gameId)
    {
        return GetActiveGame(gameId).Variant;
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

    public ActiveGameState GetGameState(string gameId)
    {
        return GetActiveGame(gameId).State;
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
        return player;
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

    public GameState GetState(string gameId)
    {
        var game = GetGame(gameId);
        return game.ExportState();
    }

    public string GetColorsAsJson(string gameId)
    {
        var activeGame = GetActiveGame(gameId);
        return activeGame.GetColorsJson();
    }

    public ColorsDTO GetColorsObject(string gameId)
    {
        var activeGame = GetActiveGame(gameId);
        return activeGame.GetColorsObject();
    }
    #endregion

    #region GameControl

    public ISet<GameEvent> Move(string move, string gameId, string playerIdentifier)
    {
        var game = GetGame(gameId);
        var player = GetPlayer(gameId, playerIdentifier);
        ISet<GameEvent> result = game.MakeMove(move, player);
        return result;
    }

    #endregion
}



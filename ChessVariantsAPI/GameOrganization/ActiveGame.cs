using ChessVariantsAPI.Hubs;
using ChessVariantsLogic;

namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// Represents an active game (not necessarily in started) which maps player identifiers to a <see cref="Player"/> type,
/// and the game they are mapped to.
/// </summary>
public class ActiveGame
{
    private Game? _game;
    private readonly Dictionary<string, Player> _playerDict;
    private string? _variant;
    public string Variant
    {
        get
        {
            if (_variant == null)
            {
                throw new GameNotFoundException("Unable to get game-variant. A game for this lobby has not been yet been set.");
            }
            return _variant;
        }
        set => _variant = value;
    }

    public ActiveGameState State { get; private set; }

    public string Admin { get; private set; }

    public ActiveGame(Game? game, string creatorPlayerIdentifier, string? variantType)
    {
        _game = game;
        _playerDict = new Dictionary<string, Player>();
        AddPlayer(creatorPlayerIdentifier);
        Admin = creatorPlayerIdentifier;
        _variant = variantType;
        State = ActiveGameState.Lobby;
    }

    public ActiveGame(string creatorPlayerIdentifier) : this(null, creatorPlayerIdentifier, null) 
    {
        State = ActiveGameState.Lobby;
    }

    public bool StartGame(string playerIdentifier)
    {
        if (playerIdentifier == Admin)
        {
            State = ActiveGameState.Game;
            return true;
        }
        return false;
    }

    public bool SetGame(Game game, string playerIdentifier, string variantType)
    {
        if (playerIdentifier != Admin)
        {
            return false;
        }
        _game = game;
        _variant = variantType;
        State = ActiveGameState.Game;
        return true;
    }

    /// <summary>
    /// Maps the <paramref name="playerIdentifier"/> to a color as supplied.
    /// </summary>
    /// <param name="playerIdentifier">The player identifier</param>
    /// <param name="color">The color to map the player identifier to</param>
    /// <returns>True if the player could be added, otherwise false</returns>
    public Player AddPlayer(string playerIdentifier)
    {
        if (_playerDict.ContainsKey(playerIdentifier))
        {
            throw new OrganizerException("You cannot join this game as you are already in it.");
        }
        var availableColors = GetAvailableColors();
        if (availableColors.Any())
        {
            var color = availableColors.First();
            _playerDict[playerIdentifier] = color;
            return color;
        }
        throw new GameFullException();
    }

    /// <summary>
    /// Returns the game
    /// </summary>
    /// <returns>returns the game</returns>
    public Game GetGame()
    {
        if (_game == null)
        {
            throw new GameNotFoundException("Unable to get game. A game for this lobby has not been yet been set.");
        }
        return _game;
    }

    /// <summary>
    /// Returns the <see cref="Player"/> enum mapped to the <paramref name="playerIdentifier"/>.
    /// If not found then returns null.
    /// </summary>
    /// <param name="playerIdentifier"></param>
    /// <returns>The player if found, otherwise null.</returns>
    public Player GetPlayer(string playerIdentifier)
    {
        try
        {
            return _playerDict[playerIdentifier];
        }
        catch (KeyNotFoundException)
        {
            throw new PlayerNotFoundException($"No player with identifier {playerIdentifier} found.");
        }
    }

    public ICollection<Tuple<string, Player>> GetPlayers()
    {
        var pairs = _playerDict.ToList();
        var players = pairs.Select(pair => Tuple.Create(pair.Key, pair.Value)).ToList();
        return players;
    }

    /// <summary>
    /// Removes a player from the active game, if they are in it. If the leaving players is admin then
    /// assigns the other player as admin 
    /// </summary>
    /// <param name="playerIdentifier">The identifier for the player to remove</param>
    /// <returns>True if the player was removed, otherwise false</returns>
    /// <exception cref="GameEmptyException">If the last player left the game</exception>
    public bool RemovePlayer(string playerIdentifier)
    {
        var success = _playerDict.Remove(playerIdentifier);
        if (success && Admin == playerIdentifier)
        {
            AssignNewAdmin();
        }

        return success;
    }

    /// <summary>
    /// Swaps the active players colors to the opposite. Only works if the requester is the admin.
    /// </summary>
    /// <param name="playerIdentifier">The player requesting the swap</param>
    public void SwapColors(string playerIdentifier)
    {
        if (playerIdentifier != Admin)
        {
            return;
        }

        foreach (var key in _playerDict.Keys)
        {
            var newColor = _playerDict[key] == Player.White ? Player.Black : Player.White;
            if (_game != null && _game.ActiveAI && _playerDict[key] == _game.GetAIColor())
            {
                _game.AssignAINewColor(newColor);
            }
            _playerDict[key] = newColor;
        }
    }

    public string GetColorsJson()
    {
        return GetColorsObject().AsJson();
    }

    public ColorsDTO GetColorsObject()
    {
        string? white = null;
        string? black = null;
        foreach (var key in _playerDict.Keys)
        {
            if (_playerDict[key] == Player.White) white = key;
            else if (_playerDict[key] == Player.Black) black = key;
        }
        return new ColorsDTO { White = white, Black = black };
    }

    private void AssignNewAdmin()
    {
        string? newAdminIdentifier = _playerDict.Keys.FirstOrDefault();
        if (newAdminIdentifier != null)
        {
            Admin = newAdminIdentifier;
        }
        else
        {
            throw new GameEmptyException();
        }
    }

    public IEnumerable<Player> GetAvailableColors()
    {
        var colors = new HashSet<Player>() { Player.White, Player.Black };
        colors.RemoveWhere(player => _playerDict.ContainsValue(player));
        return colors;
    }
}

public enum ActiveGameState
{
    Lobby,
    Game,
}

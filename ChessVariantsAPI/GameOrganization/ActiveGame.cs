using ChessVariantsLogic;

namespace ChessVariantsAPI.GameOrganization;

/// <summary>
/// Represents an active game (not necessarily in started) which maps player identifiers to a <see cref="Player"/> type,
/// and the game they are mapped to.
/// </summary>
public class ActiveGame
{
    private readonly Game _game;
    private readonly Dictionary<string, Player?> _playerDict;
    public string Variant { get; private set; }
    public string Admin { get; private set; }

    public ActiveGame(Game game, string creatorPlayerIdentifier, string variantType)
    {
        _game = game;
        _playerDict = new Dictionary<string, Player?>();
        AddPlayer(creatorPlayerIdentifier);
        Admin = creatorPlayerIdentifier;
        Variant = variantType;
    }

    /// <summary>
    /// Maps the <paramref name="playerIdentifier"/> to a color as supplied.
    /// </summary>
    /// <param name="playerIdentifier">The player identifier</param>
    /// <param name="color">The color to map the player identifier to</param>
    /// <returns>True if the player could be added, otherwise false</returns>
    public Player AddPlayer(string playerIdentifier)
    {
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
        return _game;
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
            _playerDict[key] = _playerDict[key] == Player.White ? Player.Black : Player.White;
        }
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

    private IEnumerable<Player> GetAvailableColors()
    {
        var colors = new HashSet<Player>() { Player.White, Player.Black };
        colors.RemoveWhere(player => _playerDict.ContainsValue(player));
        return colors;
    }
}
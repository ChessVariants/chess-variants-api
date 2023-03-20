namespace ChessVariantsAPI.Hubs;

public class GroupOrganizer
{
    private readonly Dictionary<string, List<string>> _groups;
    private readonly ILogger _logger;


    public GroupOrganizer(ILogger<GroupOrganizer> logger)
    {
        _groups = new Dictionary<string, List<string>>();
        _logger = logger;
    }

    public void AddToGroup(string playerIdentifier, string gameId)
    {
        if (_groups.ContainsKey(gameId))
        {
            _groups[gameId].Add(playerIdentifier);
            _logger.LogDebug("Added <{pid}> to group <{gid}>", playerIdentifier, gameId);
            return;
        }
        _groups[gameId] = new List<string>() { playerIdentifier };
    }

    public void RemoveFromGroup(string playerIdentifier, string gameId)
    {
        if (_groups.ContainsKey(gameId))
        {
            _logger.LogDebug("Removed <{pid}> from group <{gid}>", playerIdentifier, gameId);
            _groups[gameId].Remove(playerIdentifier);
        }
    }

    public ICollection<string> GetMembersOfGroup(string gameId)
    {
        if (_groups.ContainsKey(gameId))
        {
            return _groups[gameId];
        }
        return new List<string>();
    }

    public bool InGroup(string playerIdentifier, string gameId)
    {
        return GetMembersOfGroup(gameId).Contains(playerIdentifier);
    }
}

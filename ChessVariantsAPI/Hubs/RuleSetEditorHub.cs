using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using DataAccess.MongoDB;

namespace ChessVariantsAPI.Hubs;

[Authorize]
public class RuleSetEditorHub : Hub
{
    private readonly DatabaseService _db;
    private readonly ILogger<RuleSetEditorHub> _logger;

    public RuleSetEditorHub(ILogger<RuleSetEditorHub> logger, DatabaseService databaseService)
    {
        _db = databaseService;
        _logger = logger;
    }
}


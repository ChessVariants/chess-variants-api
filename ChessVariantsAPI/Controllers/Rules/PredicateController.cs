using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChessVariantsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PredicateController : GenericController
{
    private readonly ILogger<PredicateController> _logger;

    public PredicateController(DatabaseService databaseService, ILogger<PredicateController> logger) : base(databaseService)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task GetByUserName()
    {
        _logger.LogDebug("logging some");
        var username = User?.FindFirst(ClaimTypes.Name)?.Value!;
        _logger.LogInformation("User {u} requested predicates", username);
    }
}

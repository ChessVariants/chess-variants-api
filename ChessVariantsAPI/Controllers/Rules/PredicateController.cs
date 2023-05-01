using ChessVariantsAPI.DTOs;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.MongoDB.Models;

namespace ChessVariantsAPI.Controllers;

[Authorize]
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
    public async Task<IActionResult> GetByUserName()
    {
        _logger.LogDebug("logging some");
        var username = GetUsername();
        if (username == null || username == "")
        {
            _logger.LogDebug("User tried to request predicates but was either not logged in or not authorized for the action.");
            return Unauthorized("User is either not logged in or not authorized for this action.");
        }

        _logger.LogInformation("User {u} requested predicates", username);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> SavePredicate(SavePredicateDTO dto)
    {
        var pred = new Predicate
        {
            CreatorName = GetUsername(),
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
        };
        _logger.LogDebug("Trying to save pred: \n{p}", pred);
        await _db.Predicates.CreateAsync(pred);

        return Ok();
    }

    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}

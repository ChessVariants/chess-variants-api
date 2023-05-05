using ChessVariantsAPI.DTOs;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.MongoDB.Models;
using Newtonsoft.Json.Linq;

namespace ChessVariantsAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RuleSetController : GenericController
{
    private readonly ILogger<RuleSetController> _logger;

    public RuleSetController(DatabaseService databaseService, ILogger<RuleSetController> logger) : base(databaseService)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<RuleSetDTO>> GetByUserName()
    {
        _logger.LogDebug("logging some");
        var username = GetUsername();

        var ruleSets = await _db.RuleSets.FindAsync((p) => p.CreatorName == username);
        List<RuleSetDTO> ruleSetDTOs = new();
        foreach (var ruleSet in ruleSets)
        {
            ruleSetDTOs.Add(new RuleSetDTO
            {
                Name = ruleSet.Name,
                Description = ruleSet.Description,
                Predicate = ruleSet.Predicate,
                Moves = ruleSet.Moves,
                Events = ruleSet.Events,
                StalemateEvents = ruleSet.StalemateEvents,
            });
        }
        _logger.LogInformation("User {u} requested {n} rulesets", username, ruleSets.Count);
        return Ok(new RequestRuleSetDTO { RuleSets = ruleSetDTOs});
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveRuleSet(RuleSetDTO dto)
    {
        _logger.LogDebug(dto.ToString());

        var username = GetUsername();
        var rulesets = await _db.RuleSets.FindAsync((p) => p.CreatorName == username && p.Name == dto.Name);
        foreach (var ruleSet in rulesets)
        {
            await _db.RuleSets.RemoveAsync(ruleSet.Id);
        }

        var newRuleSet = new RuleSet
        {
            CreatorName = username,
            Name = dto.Name,
            Predicate = dto.Predicate,
            Description = dto.Description,
            Moves = dto.Moves,
            Events = dto.Events,
            StalemateEvents = dto.StalemateEvents,
        };
        _logger.LogDebug("Trying to save pred: \n{p}", newRuleSet);

        await _db.RuleSets.CreateAsync(newRuleSet);

        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteRuleSet(string name)
    {
        var username = GetUsername();
        var ruleSets = await _db.RuleSets.FindAsync((p) => p.CreatorName == username && p.Name == name);
        foreach(var ruleSet in ruleSets)
        {
            await _db.RuleSets.RemoveAsync(ruleSet.Id);
        }

        return Ok();
    }

    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}

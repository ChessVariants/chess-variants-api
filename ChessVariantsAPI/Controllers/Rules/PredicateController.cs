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
public class PredicateController : GenericController
{
    private readonly ILogger<PredicateController> _logger;

    public PredicateController(DatabaseService databaseService, ILogger<PredicateController> logger) : base(databaseService)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<RequestPredicateDTO>> GetByUserName()
    {
        _logger.LogDebug("logging some");
        var username = GetUsername();

        var predicates = await _db.Predicates.FindAsync((p) => p.CreatorName == username);
        List<PredicateDTO> predicateDTOs = new();
        foreach (var predicate in predicates)
        {
            predicateDTOs.Add(new PredicateDTO
            {
                Name = predicate.Name,
                Description = predicate.Description,
                Code = predicate.Code,
            });
        }
        _logger.LogInformation("User {u} requested {n} predicates", username, predicates.Count);
        return Ok(new RequestPredicateDTO { Predicates = predicateDTOs});
    }
    
    [HttpPost]
    public async Task<IActionResult> SavePredicate(SavePredicateDTO dto)
    {
        _logger.LogDebug(dto.ToString());

        var username = GetUsername();
        var predicates = await _db.Predicates.FindAsync((p) => p.CreatorName == username && p.Name == dto.Name);
        foreach (var pred_ in predicates)
        {
            await _db.Predicates.RemoveAsync(pred_.Id);
        }

        var pred = new Predicate
        {
            CreatorName = username,
            Name = dto!.Name,
            Code = dto!.Code,
            Description = dto!.Description,
        };
        _logger.LogDebug("Trying to save pred: \n{p}", pred);

        await _db.Predicates.CreateAsync(pred);

        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeletePredicate(string name)
    {
        var username = GetUsername();
        var predicates = await _db.Predicates.FindAsync((p) => p.CreatorName == username && p.Name == name);
        foreach(var pred in predicates)
        {
            await _db.Predicates.RemoveAsync(pred.Id);
        }

        return Ok();
    }

    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}

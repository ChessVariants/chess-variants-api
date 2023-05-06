
using ChessVariantsAPI.DTOs;
using ChessVariantsAPI.DTOs.Rules;
using DataAccess.MongoDB;
using DataAccess.MongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChessVariantsAPI.Controllers.Rules;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MoveController : GenericController
{
    private readonly ILogger<MoveController> _logger;

    public MoveController(DatabaseService databaseService, ILogger<MoveController> logger) : base(databaseService)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMovesByUsername()
    {
        var username = GetUsername();

        List<MoveTemplateModel> moves = await _db.Moves.FindAsync((p) => p.CreatorName == username);

        List<MoveDTO> moveDTOs = moves.Select((e) => new MoveDTO {
            Name = e.Name,
            Description = e.Description,
            Predicate = e.Predicate,
            Click = EventController.ConvertPositionRecToDTO(e.Click),
            Actions = EventController.ConvertActionRecsToDTOs(e.Actions, _logger),
            Identifier = e.Identifier,
        }).ToList();

        _logger.LogInformation("User {u} requested {n} moves", username, moveDTOs.Count);
        GetMoveDTO eDTO = new GetMoveDTO { Moves = moveDTOs };
        return Ok(eDTO);
    }


    [HttpPost]
    public async Task<IActionResult> SaveMove(MoveDTO dto)
    {
        _logger.LogDebug(dto.ToString());

        var username = GetUsername();
        var moves = await _db.Moves.FindAsync((p) => p.CreatorName == username && p.Name == dto.Name);
        foreach (var move in moves)
        {
            await _db.Moves.RemoveAsync(move.Id);
        }

        var m = new MoveTemplateModel
        {
            CreatorName = username,
            Name = dto.Name,
            Description = dto.Description,
            Predicate = dto.Predicate,
            Actions = EventController.ConvertActionDTOsToRecs(dto.Actions, _logger),
            Click = EventController.ConvertPositionDTOToRec(dto.Click),
            Identifier = dto.Identifier,
        };
        _logger.LogDebug("Trying to save move: \n{p}", m);

        await _db.Moves.CreateAsync(m);

        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteMove(string name)
    {
        var username = GetUsername();
        var moves = await _db.Moves.FindAsync((p) => p.CreatorName == username && p.Name == name);
        foreach (var move in moves)
        {
            await _db.Moves.RemoveAsync(move.Id);
        }

        return Ok();
    }

    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }


}

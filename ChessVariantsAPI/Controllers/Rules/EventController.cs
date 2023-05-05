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
public class EventController : GenericController
{
    private readonly ILogger<EventController> _logger;

    public EventController(DatabaseService databaseService, ILogger<EventController> logger) : base(databaseService)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetEventsByUsername()
    {
        var username = GetUsername();

        List<EventModel> events = await _db.Events.FindAsync((p) => p.CreatorName == username);

        List<EventDTO> eventDTOs = events.Select((e) => new EventDTO{
            Name = e.Name,
            Description = e.Description,
            Predicate = e.Predicate,
            Actions = ConvertActionRecsToDTOs(e.Actions),
        }).ToList();

        _logger.LogInformation("User {u} requested {n} events", username, eventDTOs.Count);
        GetEventDTO eDTO = new GetEventDTO { Events = eventDTOs };
        return Ok(eDTO);
    }


    [HttpPost]
    public async Task<IActionResult> SaveEvent(EventDTO dto)
    {
        _logger.LogDebug(dto.ToString());

        var username = GetUsername();
        var events = await _db.Events.FindAsync((p) => p.CreatorName == username && p.Name == dto.Name);
        foreach (var ev in events)
        {
            await _db.Events.RemoveAsync(ev.Id);
        }

        var e = new EventModel
        {
            CreatorName = username,
            Name = dto.Name,
            Description = dto.Description,
            Predicate = dto.Predicate,
            Actions = ConvertActionDTOsToRecs(dto.Actions),
        };
        _logger.LogDebug("Trying to save event: \n{p}", e);

        await _db.Events.CreateAsync(e);

        return Ok();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteEvent(string name)
    {
        var username = GetUsername();
        var events = await _db.Events.FindAsync((p) => p.CreatorName == username && p.Name == name);
        foreach (var e in events)
        {
            await _db.Events.RemoveAsync(e.Id);
        }

        return Ok();
    }


    #region HelperFunctions
    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }

    public static List<ActionDTO> ConvertActionRecsToDTOs(List<ActionRec> actions)
    {
        return actions.Select((action) => ConvertActionRecToDTO(action)).ToList();
    }

    public static ActionDTO ConvertActionRecToDTO(ActionRec actionRec)
    {
        ActionDTO result = new();
        if (actionRec.Win != null)
        {
            var winRec = actionRec.Win;
            result.Win = new WinDTO { White = winRec.WhiteWins };
        }
        else if (actionRec.Move != null)
        {
            var moveRec = actionRec.Move;
            result.Move = new MovePieceDTO { From = ConvertPositionRecToDTO(moveRec.From), To = ConvertPositionRecToDTO(moveRec.To), };
        }
        else if (actionRec.Set != null)
        {
            var setRec = actionRec.Set;
            result.Set = new SetPieceDTO { Identifier = setRec.Identifier, At = ConvertPositionRecToDTO(setRec.At) };
        }
        else if (actionRec.IsTie)
        {
            result.Tie = actionRec.IsTie;
        }
        return result;
    }

    public static PositionDTO ConvertPositionRecToDTO(Position position)
    {
        var positionDTO = new PositionDTO();
        if (position.PositionAbsolute != null)
        {
            var absoluteRec = position.PositionAbsolute;
            positionDTO.Absolute = new PositionAbsoluteDTO { Coordinate = absoluteRec.Coordinate };
        }
        else if (position.PositionRelative != null)
        {
            var relativeRec = position.PositionRelative;
            positionDTO.Relative = new PositionRelativeDTO { X = relativeRec.X, Y = relativeRec.Y, To = relativeRec.To };
        }
        return positionDTO;
    }

    public static List<ActionRec> ConvertActionDTOsToRecs(List<ActionDTO> actions)
    {
        return actions.Select((actionDTO) => ConvertActionDTOToRec(actionDTO)).ToList();
    }

    public static ActionRec ConvertActionDTOToRec(ActionDTO actionDTO)
    {
        ActionRec result = new();
        if (actionDTO.Win != null)
        {
            var winRec = actionDTO.Win;
            result.Win = new Win { WhiteWins = winRec.White };
        }
        else if (actionDTO.Move != null)
        {
            var moveRec = actionDTO.Move;
            result.Move = new MovePiece { From = ConvertPositionDTOToRec(moveRec.From), To = ConvertPositionDTOToRec(moveRec.To), };
        }
        else if (actionDTO.Set != null)
        {
            var setRec = actionDTO.Set;
            result.Set = new SetPiece { Identifier = setRec.Identifier, At = ConvertPositionDTOToRec(setRec.At) };
        }
        else if (actionDTO.Tie)
        {
            result.IsTie = actionDTO.Tie;
        }
        return result;
    }

    public static Position ConvertPositionDTOToRec(PositionDTO positionDTO)
    {
        var position = new Position();
        if (positionDTO.Absolute != null)
        {
            var absoluteRec = positionDTO.Absolute;
            position.PositionAbsolute = new PositionAbsoluteModel { Coordinate = absoluteRec.Coordinate };
        }
        else if (positionDTO.Relative != null)
        {
            var relativeRec = positionDTO.Relative;
            position.PositionRelative = new PositionRelativeModel { X = relativeRec.X, Y = relativeRec.Y, To = relativeRec.To };
        }
        return position;
    }
    #endregion
}

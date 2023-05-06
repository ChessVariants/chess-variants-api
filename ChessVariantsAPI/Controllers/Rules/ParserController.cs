using ChessVariantsAPI.DTOs;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.MongoDB.Models;
using Newtonsoft.Json.Linq;
using ChessVariantsAPI.DTOs.Rules;
using System.ComponentModel.DataAnnotations;
using ChessVariantsLogic.Rules.Predicates;
using Newtonsoft.Json;

namespace ChessVariantsAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ParserController : GenericController
{
    private readonly ILogger<ParserController> _logger;
    private readonly PredicateParser pp;

    public ParserController(DatabaseService databaseService, ILogger<ParserController> logger) : base(databaseService)
    {
        _logger = logger;
        pp = new PredicateParser();
    }

    [HttpPost]
    public async Task<IActionResult> CompileCode(CodeDTO dto)
    {
        string code = dto.Code;
        string exception = "Compile Successful.";
        try
        {
            _logger.LogDebug("Predicate compiled successfully: " + JsonConvert.SerializeObject(PredicateParser.ParseCode(code)));
        }
        catch (PrediChessException ex)
        {
            exception = ex.Message;
        }
        catch(Exception ex)
        {
            exception = "An unexpected error happened.";
            _logger.LogDebug("Exception found in predicate parser: " + exception + ", " + ex.StackTrace);
        }

        return Ok( new ExceptionDTO { Exception = exception });
    }

    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}

public record CodeDTO
{
    [Required]
    public string Code { get; set; } = null!;
}
public record ExceptionDTO
{
    [Required]
    public string Exception { get; set; } = null!;
    [Required]
    public bool CompiledCorrectly { get; set; } = false;
}

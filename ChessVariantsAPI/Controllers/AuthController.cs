using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : GenericController
{
    ILogger _logger;

    public AuthController(DatabaseService databaseService, ILogger<AuthController> logger) : base(databaseService)
    {
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Authenticate()
    {
        _logger.LogDebug("Authenticated user");
        return Ok();
    }

}

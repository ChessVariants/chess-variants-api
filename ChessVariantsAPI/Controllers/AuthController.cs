using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : GenericController
{
    public AuthController(DatabaseService databaseService, ILogger<UsersController> logger) : base(databaseService, logger)
    {
    }

    [Authorize]
    [HttpGet]
    public IActionResult Authenticate()
    {
        _logger.LogDebug("Authenticated user");
        return Ok();
    }

}

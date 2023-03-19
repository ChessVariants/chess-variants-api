using DataAccess.MongoDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public abstract class GenericController : ControllerBase
{
    readonly protected DatabaseService _db;
    readonly protected ILogger _logger;

    public GenericController(DatabaseService databaseService, ILogger<UsersController> logger)
    {
        _db = databaseService;
        _logger = logger;
    }
}

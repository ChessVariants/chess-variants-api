using DataAccess.MongoDB;
using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsAPI.Controllers;

/// <summary>
/// This class is an abstract implementation of functionality useful for all controllers, such as querying the database and logging.
/// </summary>
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

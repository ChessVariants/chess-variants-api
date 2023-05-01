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

    public GenericController(DatabaseService databaseService)
    {
        _db = databaseService;
    }
}

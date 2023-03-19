using ChessVariantsAPI.Authentication;
using ChessVariantsAPI.DTOs;
using DataAccess.MongoDB;
using DataAccess.MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ChessVariantsAPI.Controllers;

/// <summary>
/// This controller exposes endpoints for handling users, i.e creating a new user.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UsersController : GenericController
{
    public UsersController(DatabaseService databaseService, ILogger<UsersController> logger) : base(databaseService, logger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get() // TODO: remove
    {
        var users = await _db.Users.GetAsync();
        _logger.LogInformation("Get request found {amount} users", users.Count);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<CreatedUserDTO>> CreateUser(CreateUserDTO createUserDTO)
    {
        _logger.LogInformation("Creating user: {user}", createUserDTO);
        var passwordHasher = PasswordHasherFactory.SHA256();
        var passwordSalt = passwordHasher.GenerateSalt();
        var hashedPassword = passwordHasher.Hash(createUserDTO.Password, passwordSalt);

        var user = new User { 
            Username = createUserDTO.Username,
            Email = createUserDTO.Email,
            PasswordHash = hashedPassword,
            PasswordSalt = passwordSalt
        };

        try
        {
            await _db.Users.CreateAsync(user);
        }
        catch (MongoException e)
        {
            _logger.LogError("Writing user {u} error: {e}", createUserDTO, e.Message);
            return BadRequest("An Unexpected error occured");
        }

        createUserDTO.Password = "";
        return CreatedAtAction("Get", new CreatedUserDTO { Email = createUserDTO.Email, Username = createUserDTO.Email });
    }
}

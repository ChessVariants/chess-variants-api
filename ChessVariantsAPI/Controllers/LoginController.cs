using ChessVariantsAPI.Authentication;
using ChessVariantsAPI.DTOs;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsAPI.Controllers;

/// <summary>
/// This controller exposes endpoints for logging in, either via a previously created account or as a guest, which gives the caller a temporary JWT.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class LoginController : GenericController
{
    private readonly JWTUtils _jwtUtils;
    ILogger _logger;

    public LoginController(DatabaseService databaseService, ILogger<LoginController> logger, JWTUtils jwtUtils) : base(databaseService)
    {
        _jwtUtils = jwtUtils;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<LoggedInUserDTO> LoginAsGuest()
    {
        var username = "Guest-" + Guid.NewGuid().ToString();
        var token = _jwtUtils.GenerateToken(username, email: null, expirationDays: 1);
        _logger.LogDebug("Created guest account: {guest}, token: {t}", username, token);
        return Ok(new LoggedInUserDTO { Username = username, Email = "", Token = token });
    }

    [HttpPost]
    public async Task<ActionResult<LoggedInUserDTO>> Login(LoginDTO loginDTO)
    {
        _logger.LogInformation("Logging in user {user}", loginDTO.Email);

        var existingUser = await _db.Users.GetByEmailAsync(loginDTO.Email);
        if (existingUser == null)
        {
            _logger.LogInformation("Login email not found: ", loginDTO.Email);
            return Unauthorized("Invalid email or password");
        }

        bool verified = PasswordVerifier.Verify(
            loginDTO.Password,
            existingUser.PasswordHash,
            existingUser.PasswordSalt,
            PasswordHasherFactory.SHA256());

        if (!verified)
        {
            _logger.LogInformation("User {email} tried to log in with wrong password", loginDTO.Email);
            return Unauthorized("Invalid email or password");
        }

        var token = _jwtUtils.GenerateToken(existingUser.Username, existingUser.Email);
        return Ok(new LoggedInUserDTO { Username = existingUser.Username, Email = existingUser.Email, Token = token});

    }
}

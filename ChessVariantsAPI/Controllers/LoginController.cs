using ChessVariantsAPI.Authentication;
using ChessVariantsAPI.DTOs;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LoginController : GenericController
{
    private readonly JWTUtils _jwtUtils;

    public LoginController(DatabaseService databaseService, ILogger<UsersController> logger, JWTUtils jwtUtils) : base(databaseService, logger)
    {
        _jwtUtils = jwtUtils;
    }

    [HttpGet]
    public IActionResult LoginAsGuest()
    {
        var username = "Guest-" + Guid.NewGuid().ToString();
        var token = _jwtUtils.GenerateToken(username, email: null, expirationDays: 1);
        _logger.LogDebug("Created guest account: {guest}, token: {t}", username, token);
        return Ok(new { username, JTWToken = token });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
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
        return Ok(new { existingUser.Username, existingUser.Email, JTWToken = token });

    }
}

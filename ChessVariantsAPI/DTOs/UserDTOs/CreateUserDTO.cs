using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs;

public record CreateUserDTO
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

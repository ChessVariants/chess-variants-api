using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs;

public record LoginDTO
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

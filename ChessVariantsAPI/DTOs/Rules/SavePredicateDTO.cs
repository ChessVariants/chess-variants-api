using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs;

public record SavePredicateDTO
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Code { get; set; } = null!;
}

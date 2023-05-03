using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs;

public record DeletePredicateDTO
{
    [Required]
    public string Name { get; set; } = null!;
}

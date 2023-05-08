using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs.VariantDTOs;

public record CreateVariantDTO
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string WhiteRuleSetIdentifier { get; set; } = null!;

    [Required]
    public string BlackRuleSetIdentifier { get; set; } = null!;

    [Required]
    public string BoardIdentifier { get; set; } = null!;

    [Required]
    public int MovesPerTurn { get; set; } = 1;
}

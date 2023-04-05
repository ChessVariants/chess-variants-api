using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs;

public record CreateVariantDTO
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string Creator { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string VariantData { get; set; } = null!;
}

public record CreatedVariantDTO
{
    [Required]
    public string Id { get; set; } = null!;
}

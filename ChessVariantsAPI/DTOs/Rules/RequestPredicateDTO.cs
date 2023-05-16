using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs;

public record RequestPredicateDTO
{
    public List<PredicateDTO> Predicates { get; set; } = null!;
}

public record PredicateDTO
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Code { get; set; } = null!;
}

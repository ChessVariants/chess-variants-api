using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs.Rules;

public record GetMoveDTO
{
    [Required]
    public List<MoveDTO> Moves { get; set; } = null!;
}

public record MoveDTO
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public string Predicate { get; set; } = null!;
    [Required]
    public List<ActionDTO> Actions { get; set; } = null!;
    [Required]
    public string Identifier { get; set; } = null!;
    [Required]
    public PositionDTO Click { get; set; } = null!;

}


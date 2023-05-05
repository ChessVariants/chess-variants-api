using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs.Rules;

public record GetEventDTO
{
    [Required]
    public List<EventDTO> Events { get; set; } = null!;
}

public record EventDTO
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public string Predicate { get; set; } = null!;
    [Required]
    public List<ActionDTO> Actions {get; set; } = null!;
    
}

public record ActionDTO
{
    public WinDTO? Win { get; set; } = null;
    public SetPieceDTO? Set { get; set; } = null;
    public MovePieceDTO? Move { get; set; } = null;
    public bool Tie = false;
}

public record WinDTO
{
    [Required]
    public bool? White { get; set; } = null;
}

public record SetPieceDTO
{
    [Required]
    public string Identifier { get; set; } = null!;
    [Required]
    public PositionDTO At { get; set; } = null!;
}
public record MovePieceDTO
{
    [Required]
    public PositionDTO From { get; set; } = null!;
    [Required]
    public PositionDTO To { get; set; } = null!;
}

public record PositionDTO
{
    public PositionAbsoluteDTO? Absolute { get; set; } = null!;
    public PositionRelativeDTO? Relative { get; set; } = null!;
}

public record PositionAbsoluteDTO
{
    [Required]
    public string Coordinate { get; set; } = null!;
}


public record PositionRelativeDTO
{
    [Required]
    public int? X { get; set; } = null;
    [Required]
    public int? Y { get; set; } = null;
    [Required]
    public bool? To { get; set; } = null;
}


using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs;

public record RequestRuleSetDTO
{
    public List<RuleSetDTO> RuleSets { get; set; } = null!;
}
public record RuleSetDTO
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public string Predicate { get; set; } = null!;
    [Required]
    public List<string> Moves { get; set; } = null!;
    [Required]
    public List<string> Events { get; set; } = null!;
    [Required]
    public List<string> StalemateEvents { get; set; } = null!;
}

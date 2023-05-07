using Newtonsoft.Json;

namespace ChessVariantsAPI.Hubs.DTOs;

public record PieceDTO
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("image")]
    public string Image { get; set; } = null!;

    [JsonProperty("color")]
    public string Color { get; set; } = null!;
}
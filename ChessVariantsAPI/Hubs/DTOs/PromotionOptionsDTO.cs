using System;
using Newtonsoft.Json;

namespace ChessVariantsAPI.Hubs.DTOs;

public record PromotionOptionsDTO
{
    [JsonProperty("promotablePieces")]
    public List<string> PromotablePieces { get; set; } = null!;

    [JsonProperty("player")]
    public string Player { get; set; } = null!;
}

public record PieceInfo
{
    [JsonProperty("identifier")]
    public string Identifier { get; set; } = null!;

    [JsonProperty("imagePath")]
    public string ImagePath { get; set; } = null!;
}

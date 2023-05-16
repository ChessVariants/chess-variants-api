using Newtonsoft.Json;

namespace ChessVariantsLogic.Export;

/// <summary>
/// This class exports an object of type <see cref="Piece"/> to a string of Json-format.
/// </summary>
public static class PieceExporter
{
  
}

/// <summary>
/// Represents a Json-object of an <see cref="PatternRecord"/>.
/// </summary>
public record PatternRecord
{
    [JsonProperty("xDir")]
    public int XDir { get; set; }

    [JsonProperty("yDir")]
    public int YDir { get; set; }

    [JsonProperty("minLength")]
    public int MinLength { get; set; }

    [JsonProperty("maxLength")]
    public int MaxLength { get; set; }
    
}
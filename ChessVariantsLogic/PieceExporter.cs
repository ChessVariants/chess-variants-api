using Newtonsoft.Json;

namespace ChessVariantsLogic.Export;

/// <summary>
/// This class exports an object of type <see cref="Piece"/> to a string of Json-format.
/// </summary>
public static class PieceExporter
{
    /// <summary>
    /// Exports <paramref name="piece"/> of type <see cref="Piece"/> to a string of Json-format.
    /// </summary>
    /// <param name="piece"> is the piece that should be exported.</param>
    /// <returns>A string in Json-format of <paramref name="piece"/>.</returns>
    public static string ExportPieceStateAsJson(Piece piece)
    {
        return ExportPieceState(piece).AsJson();
    }

    /// <summary>
    /// Exports <paramref name="piece"/> into a <see cref="PieceState"/> object.
    /// </summary>
    /// <param name="piece">is the piece that should be exported.</param>
    /// <returns>A <see cref="PieceState"/> of the <paramref name="piece"/>.</returns>
    public static PieceState ExportPieceState(Piece piece)
    {
        var pieceState = new PieceState
        {
            Movement = exportPattern(piece.GetAllMovementPatterns()),
            Captures = exportPattern(piece.GetAllCapturePatterns()),
            Royal = piece.Royal,
            CanBeCaptured = piece.CanBeCaptured,
            PieceClassifier = piece.PieceClassifier.AsString(),
            PieceIdentifier = piece.PieceIdentifier,
            Repeat = piece.Repeat,
        };
        return pieceState;
    }

    private static List<PatternRecord> exportPattern(IEnumerable<Pattern> movement)
    {
        var patterns = new List<PatternRecord>();
        foreach (var p in movement)
        {
            patterns.Add(new PatternRecord
            {
                XDir = p.XDir,
                YDir = p.YDir,
                MinLength = p.MinLength,
                MaxLength = p.MaxLength,
            });
        }
        return patterns;
    }

}

/// <summary>
/// Represents an exportation of the state of a piece and
/// can be exported to a JSON formatted string for communication between entities.
/// </summary>
public record PieceState
{
    [JsonProperty("movement")]
    public List<PatternRecord> Movement { get; set; } = null!;

    [JsonProperty("captures")]
    public List<PatternRecord> Captures { get; set; } = null!;

    [JsonProperty("royal")]
    public bool Royal { get; set; }

    [JsonProperty("pieceClassifier")]
    public string PieceClassifier { get; set; } = null!;

    [JsonProperty("repeat")]
    public int Repeat { get; set; }

    [JsonProperty("pieceIdentifier")]
    public string PieceIdentifier { get; set; } = null!;

    [JsonProperty("canBeCaptured")]
    public bool CanBeCaptured { get; set; }

    public string AsJson()
    {
        return JsonConvert.SerializeObject(this,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
    }
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
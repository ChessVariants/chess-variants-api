using Newtonsoft.Json;

namespace ChessVariantsLogic.Export;

public static class PieceExporter
{
    public static string ExportPieceStateAsJson(Piece piece)
    {
        return ExportPieceState(piece).AsJson();
    }

    public static PieceState ExportPieceState(Piece piece)
    {
        var pieceState = new PieceState
        {
            Movement = exportPattern(piece.GetAllMovementPatterns()),
            Captures = exportPattern(piece.GetAllCapturePatterns()),
            Royal = piece.Royal,
            CanBeCaptured = piece.CanBeCaptured,
            PieceClassifier = piece.PieceClassifier,
            PieceIdentifier = piece.PieceIdentifier,
            Repeat = piece.Repeat,
        };
        return pieceState;
    }

    private static List<Pattern> exportPattern(IEnumerable<IPattern> movement)
    {
        var patterns = new List<Pattern>();
        foreach (var p in movement)
        {
            patterns.Add(new Pattern
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

public record PieceState
{
    [JsonProperty("movements")]
    public List<Pattern> Movement { get; set; } = null!;

    [JsonProperty("captures")]
    public List<Pattern> Captures { get; set; } = null!;

    [JsonProperty("royal")]
    public bool Royal { get; set; }

    [JsonProperty("belongs_to")]
    public PieceClassifier PieceClassifier { get; set; }

    [JsonProperty("repeat")]
    public int Repeat { get; set; }

    [JsonProperty("identifier")]
    public string PieceIdentifier { get; set; } = null!;

    [JsonProperty("capturable")]
    public bool CanBeCaptured { get; set; }

    public string AsJson()
    {
        return JsonConvert.SerializeObject(this,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
    }
}

public record Pattern
{
    [JsonProperty("x_dir")]
    public int XDir { get; set; }

    [JsonProperty("y_dir")]
    public int YDir { get; set; }

    [JsonProperty("min_length")]
    public int MinLength { get; set; }

    [JsonProperty("max_length")]
    public int MaxLength { get; set; }
    
}
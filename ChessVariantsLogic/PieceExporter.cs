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
            PieceClassifier = piece.PieceClassifier,
            PieceIdentifier = piece.PieceIdentifier,
            Repeat = piece.Repeat,
        };
        return pieceState;
    }

    public static string ExportLegalMovesAsJson(HashSet<string> moveSet)
    {
        var moveDict = new Dictionary<string, List<string>>();

        foreach (var move in moveSet)
        {
            var fromTo = MoveWorker.ParseMove(move);
            if (fromTo == null)
                throw new InvalidOperationException($"Could not parse move {move}");

            var moveList = moveDict.GetValueOrDefault(fromTo.Item1, new List<string>());
            if (moveList.Count == 0)
            {
                moveDict[fromTo.Item1] = moveList;
            }
            moveList.Add(fromTo.Item2);
        }
        return GameExporter.ExportMovesAsJson(moveDict);
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

/// <summary>
/// Represents an exportation of the state of a piece and
/// can be exported to a JSON formatted string for communication between entities.
/// </summary>
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

/// <summary>
/// Represents a Json-object of an <see cref="IPattern"/>.
/// </summary>
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
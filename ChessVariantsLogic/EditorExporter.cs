using Newtonsoft.Json;

namespace ChessVariantsLogic.Export;

public class EditorExporter
{

    public static PatternState ExportPatternState(MovementPattern pattern)
    {
        var list = new List<PatternRecord>();
        foreach(var p in pattern.GetAllPatterns())
        {
            list.Add(new PatternRecord
            {
                XDir = p.XDir,
                YDir = p.YDir,
                MinLength = p.MinLength,
                MaxLength = p.MaxLength,
            });
        }
        var state = new PatternState
        {
            Patterns = list,
        };
        return state;
    }

    public static EditorState ExportEditorState(Chessboard chessboard, Player sideToMove, HashSet<string> moves, string square)
    {
        var gameState = GameExporter.ExportGameState(chessboard, sideToMove, getLegalMovesDict(moves));
        var editorState = new EditorState
        {
            Board = gameState.Board,
            BoardSize = gameState.BoardSize,
            Moves = gameState.Moves,
            Square = square,
        };
        return editorState;
    }

    private static Dictionary<string, List<string>> getLegalMovesDict(HashSet<string> moveSet)
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
        return moveDict;
    }
}

public record EditorState
{

    [JsonProperty("board")]
    public List<string> Board { get; set; } = null!;

    [JsonProperty("boardSize")]
    public BoardSize BoardSize { get; set; } = null!;

    [JsonProperty("moves")]
    public List<MoveRecord> Moves { get; set; } = null!;

    [JsonProperty("square")]
    public string Square { get; set; } = null!;

    public string AsJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

public record PatternState
{
    [JsonProperty("patterns")]
    public List<PatternRecord> Patterns {get; set; } = null!;
        
    public string AsJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
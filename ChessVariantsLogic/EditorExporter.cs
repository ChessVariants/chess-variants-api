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

    public static BoardEditorState ExportBoardEditorState(MoveWorker mw)
    {
        return new BoardEditorState
        {
            Board = GameExporter.ExportBoard(mw),
            BoardSize = new BoardSize { Rows = mw.Board.Rows, Cols = mw.Board.Cols }
        };
    }

    public static PieceEditorState ExportPieceEditorState(MoveWorker mw, Player sideToMove, HashSet<string> moves, string square)
    {
        var gameState = GameExporter.ExportGameState(mw, sideToMove, getLegalMovesDict(moves));
        var editorState = new PieceEditorState
        {
            Board = gameState.Board,
            BoardSize = gameState.BoardSize,
            Moves = gameState.Moves,
            Square = square,
            BelongsTo = gameState.SideToMove,
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

public record PieceImageState
{
    [JsonProperty("board")]
    public List<string> Board { get; set; } = null!;

    [JsonProperty("boardSize")]
    public BoardSize BoardSize { get; set; } = null!;
}

public record BoardEditorState
{
    [JsonProperty("board")]
    public List<string> Board { get; set; } = null!;

    [JsonProperty("boardSize")]
    public BoardSize BoardSize { get; set; } = null!;
}

public record PieceEditorState
{

    [JsonProperty("board")]
    public List<string> Board { get; set; } = null!;

    [JsonProperty("boardSize")]
    public BoardSize BoardSize { get; set; } = null!;

    [JsonProperty("moves")]
    public List<MoveRecord> Moves { get; set; } = null!;

    [JsonProperty("square")]
    public string Square { get; set; } = null!;

    [JsonProperty("belongsTo")]
    public string BelongsTo { get; set; } = null!;

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
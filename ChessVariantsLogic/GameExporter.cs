using Newtonsoft.Json;
using static ChessVariantsLogic.Game;
namespace ChessVariantsLogic.Export;
public static class GameExporter
{
    public static string ExportGameStateAsJson(MoveWorker mv, Player sideToMove, Dictionary<string, List<string>> moveDict)
    {
        var gameState = ExportGameState(mv, sideToMove, moveDict);
        return gameState.AsJson();
    }

    public static string ExportMovesAsJson(Dictionary<string, List<string>> moveDict)
    {
        var moves = ExportMoves(moveDict);
        return JsonConvert.SerializeObject(moves, Formatting.Indented);
    }


    /// <summary>
    /// Gives a <see cref="GameState"/> object representing the current state of the game
    /// </summary>
    /// <param name="chessboard">The board to export</param>
    /// <param name="sideToMove">The side whose turn it is to move</param>
    /// <param name="moveDict">Moves to export</param>
    /// <returns></returns>
    public static GameState ExportGameState(
        MoveWorker mw,
        Player sideToMove,
        Dictionary<string, List<string>> moveDict,
        int? lastestMoveFromIndex = null,
        int? lastestMoveToIndex = null)
    {
        return new GameState
        {
            SideToMove = sideToMove.AsString(),
            Board = ExportBoard(mw),
            BoardSize = new BoardSize { Rows = mw.Board.Rows, Cols = mw.Board.Cols },
            Moves = ExportMoves(moveDict),
            LatestMoveFromIndex = lastestMoveFromIndex,
            LatestMoveToIndex = lastestMoveToIndex,
        };
    }

    private static List<MoveRecord> ExportMoves(Dictionary<string, List<string>> moveDict)
    {
        var moves = new List<MoveRecord>();
        foreach (var move in moveDict)
        {
            moves.Add(new MoveRecord { From = move.Key, To = move.Value });
        }
        return moves;
    }

    public static List<string> ExportBoard(MoveWorker mw)
    {
        var boardPieces = new List<string>();
        int sameConsecutivePieceCount = 1;
        string previousPieceIdentifier = "";

        foreach (var pos in mw.Board.GetAllCoordinates())
        {
            string pieceIdentifier = TryGetPieceIdentifier(mw, pos);

            if (pieceIdentifier == previousPieceIdentifier)
            {
                sameConsecutivePieceCount++;
            }
            else
            {
                AddPieces(boardPieces, sameConsecutivePieceCount, previousPieceIdentifier);
                sameConsecutivePieceCount = 1;
            }
            previousPieceIdentifier = pieceIdentifier;
        }

        AddPieces(boardPieces, sameConsecutivePieceCount, previousPieceIdentifier);
        return boardPieces;
    }

    private static void AddPieces(List<string> boardPieces, int sameConsecutivePieceCount, string previousPieceIdentifier)
    {
        if (previousPieceIdentifier == "")
        {
            return;
        }
        var count = sameConsecutivePieceCount > 1 ? $"{sameConsecutivePieceCount}" : "";
        boardPieces.Add($"{previousPieceIdentifier}{count}");
    }

    private static string TryGetPieceIdentifier(MoveWorker mw, (int, int) pos)
    {
        //var pieceIdentifier = mw.Board.GetPieceIdentifier(pos.Item1, pos.Item2);
        var pieceIdentifier = mw.Board.GetPieceImagePath(mw, pos.Item1, pos.Item2);
        if (pieceIdentifier == null)
        {
            throw new InvalidOperationException($"Position {pos} was eligible for board of size {mw.Board.Rows}x{mw.Board.Cols}");
        }
        return pieceIdentifier;
    }
}

/// <summary>
/// Represents an exportation of the state of the game and
/// can be exported to a JSON formatted string. for communication between entities.
/// </summary>
public record GameState
{
    [JsonProperty("sideToMove")]
    public string SideToMove { get; set; } = null!;

    [JsonProperty("board")]
    public List<string> Board { get; set; } = null!;

    [JsonProperty("boardSize")]
    public BoardSize BoardSize { get; set; } = null!;

    [JsonProperty("moves")]
    public List<MoveRecord> Moves { get; set; } = null!;

    [JsonProperty("latestMoveFromIndex")]
    public int? LatestMoveFromIndex { get; set; } = null!;

    [JsonProperty("latestMoveToIndex")]
    public int? LatestMoveToIndex { get; set; } = null!;

    public string AsJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

/// <summary>
/// Represents a json-object of the board size
/// </summary>
public record BoardSize
{
    [JsonProperty("rows")]
    public int Rows { get; set; }

    [JsonProperty("cols")]
    public int Cols { get; set; }
}

/// <summary>
/// Represents a json-object of which moves a piece can make.
/// </summary>
public record MoveRecord
{
    [JsonProperty("from")]
    public string From { get; set; } = null!;

    [JsonProperty("to")]
    public IEnumerable<string> To { get; set; } = null!;
}
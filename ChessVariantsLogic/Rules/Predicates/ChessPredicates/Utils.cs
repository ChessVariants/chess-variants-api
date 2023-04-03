using System.Numerics;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// Utility methods used by to get information about the game. Should be moved eventually.
/// </summary>
public class Utils
{

    /// <summary>
    /// Returns whether a piece with identifier <paramref name="pieceIdentifier"/> is attacked or not.
    /// </summary>
    /// <param name="board">The board state which you want to find information about</param>
    /// <param name="pieceIdentifier">The identifier for the piece you're trying to see if attacked</param>
    /// <returns>True if a piece with the supplied <paramref name="pieceIdentifier"/> is attacked, otherwise false.</returns>
    public static bool PieceAttacked(MoveWorker board, string pieceIdentifier)
    {
        var player = GetPieceClassifier(board, pieceIdentifier);
        var attacker = player == PieceClassifier.WHITE ? Player.Black : Player.White;
        var piecePositions = FindPiecesOfType(board, pieceIdentifier);
        var attackedPieces = board.GetAllCapturePatternMoves(attacker);
        foreach (var attackerMove in attackedPieces)
        {
            var (_, to) = MoveWorker.ParseMove(attackerMove);
            if (piecePositions.Contains(to))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Returns whether a square with coordinate <paramref name="position"/> is attacked or not.
    /// </summary>
    /// <param name="board">The board state which you want to find information about</param>
    /// <param name="position">The position of the square you're trying to see if attacked</param>
    /// <param name="attacker">The side that's attacking the square</param>
    /// <returns>True if the square with coordinate <paramref name="position"/> is attacked by <paramref name="attacker"/> </returns>
    public static bool SquareAttacked(MoveWorker board, string position, Player attacker)
    {
        var attackedPieces = board.GetAllCapturePatternMoves(attacker);
        foreach (var attackerMove in attackedPieces)
        {
            var (_, to) = MoveWorker.ParseMove(attackerMove);
            if (position.Equals(to))
            {
                return true;
            }
        }
        return false;
    }

    public static PieceClassifier GetPieceClassifier(MoveWorker board, string pieceIdentifier)
    {
        if (pieceIdentifier == "BLACK")
            return PieceClassifier.BLACK;
        if (pieceIdentifier == "WHITE")
            return PieceClassifier.WHITE;
        if (pieceIdentifier == "SHARED")
            return PieceClassifier.SHARED;

        return board.GetPieceClassifier(pieceIdentifier);
    }

    public static bool IsOfType(Piece? piece, string pieceIdentifier)
    {
        if (piece == null)
            return false;
        if (pieceIdentifier == "ANY")
            return true;
        if (pieceIdentifier == "BLACK")
            return piece.PieceClassifier.Equals(PieceClassifier.BLACK);
        if (pieceIdentifier == "WHITE")
            return piece.PieceClassifier.Equals(PieceClassifier.WHITE);
        if (pieceIdentifier == "SHARED")
            return piece.PieceClassifier.Equals(PieceClassifier.SHARED);

        return piece.PieceIdentifier == pieceIdentifier;
    }

    /// <summary>
    /// Returns all positions where a piece with <paramref name="pieceIdentifier"/> are located.
    /// </summary>
    /// <param name="board">The board to find pieces on</param>
    /// <param name="pieceIdentifier">The identifier for the piece type whose locations to find</param>
    /// <returns>All positions where a piece with <paramref name="pieceIdentifier"/> are located.</returns>
    public static IEnumerable<string> FindPiecesOfType(MoveWorker board, string pieceIdentifier)
    {
        var pieceLocations = new List<string>();
        foreach (var position in board.Board.CoorToIndex.Keys)
        {
            string? pieceId = board.Board.GetPieceIdentifier(position);
            if (pieceId == null || pieceId == Constants.UnoccupiedSquareIdentifier) continue;
            var piece = board.GetPieceFromIdentifier(pieceId);
            if (IsOfType(piece, pieceIdentifier))
            {
                pieceLocations.Add(position);
            }
        }
        return pieceLocations;
    }

}
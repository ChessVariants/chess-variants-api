using ChessVariantsLogic;
using ChessVariantsLogic.Predicates;
using System;
using System.Numerics;

public class Checked : IPredicate
{
    private BoardState boardState;
    private string pieceIdentifier;

    public Checked(BoardState boardState, string pieceIdentifier)
	{
        this.boardState = boardState;
        this.pieceIdentifier = pieceIdentifier;
    }

    public bool evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        Chessboard board = boardState == BoardState.NEXT ? nextBoardState : thisBoardState;

        var royalAttacked = PieceChecked(board, pieceIdentifier);
        return royalAttacked;
    }
    static string getPlayer(string pieceIdentifier)
    {
        return "black";
    }

    private static bool PieceChecked(Chessboard board, string pieceIdentifier)
    {
        string player = getPlayer(pieceIdentifier);
        string attacker = player == "white" ? "black" : "white";
        var piecePositions = FindPiecesOfType(board, pieceIdentifier);
        foreach (var attackerMove in board.GetAllMoves(attacker))
        {
            var (_, to) = board.parseMove(attackerMove);
            if (piecePositions.Contains(to))
            {
                return true;
            }
        }
        return false;
    }

    private static IEnumerable<string> FindPiecesOfType(Chessboard board, string pieceIdentifier)
    {
        var pieceLocations = new List<string>();
        foreach (var position in board.CoorToIndex.Keys)
        {
            if (IsOfType(position, board, pieceIdentifier))
            {
                pieceLocations.Add(position);
            }
        }
        return pieceLocations;
    }

    private static bool IsOfType(string position, Chessboard board, string pieceIdentifier)
    {
        var piece = board.GetPiece(position);
        switch (pieceIdentifier)
        {
            // TODO "ANY_BLACK", "ANY_WHITE" instead
            case "ANY":
                return piece != Constants.UnoccupiedSquareIdentifier;
            case "ROYAL":
                return piece == Constants.BlackKingIdentifier;
            default:
                return piece == pieceIdentifier;
        }
    }


}

public static class PieceType
{
    public static readonly string ROYAL = "ROYAL";
    public static readonly string ANY = "ANY";
}

public enum BoardState
{
    THIS, NEXT, ALL_NEXT
}
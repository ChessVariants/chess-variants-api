using ChessVariantsLogic;
using ChessVariantsLogic.Predicates;
using System;
using System.Numerics;

public class Checked : IPredicate
{
    private BoardState boardState;
    private string pieceIdentifier;
    private string player;

    public Checked(BoardState boardState, string pieceIdentifier, string player)
	{
        this.boardState = boardState;
        this.pieceIdentifier = pieceIdentifier;
        this.player = player;
    }

    public bool evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        Chessboard board = boardState == BoardState.NEXT ? nextBoardState : thisBoardState;

        string opponent = player == "black" ? "white" : "black";

        var royalAttacked = PieceChecked(board, player, opponent, pieceIdentifier);
        return royalAttacked;
    }

    private static bool PieceChecked(Chessboard board, string sideToPlay, string attacker, string pieceIdentifier)
    {
        var piecePositions = FindPiecesOfType(board, sideToPlay, pieceIdentifier);
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

    private static IEnumerable<string> FindPiecesOfType(Chessboard board, string player, string pieceIdentifier)
    {
        var pieceLocations = new List<string>();
        foreach (var position in board.CoorToIndex.Keys)
        {
            if (IsOfType(position, board, player, pieceIdentifier))
            {
                pieceLocations.Add(position);
            }
        }
        return pieceLocations;
    }

    private static bool IsOfType(string position, Chessboard board, string player, string pieceIdentifier)
    {
        var piece = board.GetPiece(position);
        switch (pieceIdentifier)
        {
            // TODO "ANY_BLACK", "ANY_WHITE" instead
            case "ANY":
                return piece != Constants.UnoccupiedSquareIdentifier;
            case "ROYAL":
            {
                if (player == "black")
                {
                    return piece == Constants.BlackKingIdentifier;
                }
                return piece == Constants.WhiteKingIdentifier;
            }
            default: return piece == pieceIdentifier;
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
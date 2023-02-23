using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Predicates;
internal class PiecesLeft : IPredicate
{
    Comparator comparator;
    int compareValue;
    BoardState state;
    string pieceIdentifier;

    public PiecesLeft(Comparator comparator, int compareValue, BoardState state, string pieceIdentifier)
    {
        this.comparator = comparator;
        this.compareValue = compareValue;
        this.state = state;
        this.pieceIdentifier = pieceIdentifier;
    }

    public bool evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        Chessboard board = state == BoardState.THIS ? thisBoardState : nextBoardState;
        int piecesLeft = FindPiecesOfType(board, pieceIdentifier).Count();
        return piecesLeft == compareValue;
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

public enum Comparator {
    GREATER_THAN, LESS_THAN, GREATER_THAN_OR_EQUALS, LESS_THAN_OR_EQUALS, EQUALS, NOT_EQUALS
}


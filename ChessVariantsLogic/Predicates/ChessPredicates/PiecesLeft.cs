using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Predicates;
public class PiecesLeft : IPredicate
{
    private readonly Comparator _comparator;
    private readonly int _compareValue;
    private readonly BoardState _state;
    private readonly string _pieceIdentifier;

    public PiecesLeft(Comparator comparator, int compareValue, BoardState state, string pieceIdentifier)
    {
        _comparator = comparator;
        _compareValue = compareValue;
        _state = state;
        _pieceIdentifier = pieceIdentifier;
    }

    public bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        Chessboard board = _state == BoardState.THIS ? thisBoardState : nextBoardState;
        int piecesLeft = Utils.FindPiecesOfType(board, _pieceIdentifier).Count();
        return piecesLeft == _compareValue;
    }

}

public enum Comparator {
    GREATER_THAN, LESS_THAN, GREATER_THAN_OR_EQUALS, LESS_THAN_OR_EQUALS, EQUALS, NOT_EQUALS
}


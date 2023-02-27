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

    public PiecesLeft(string pieceIdentifier, Comparator comparator, int compareValue, BoardState state)
    {
        _comparator = comparator;
        _compareValue = compareValue;
        _state = state;
        _pieceIdentifier = pieceIdentifier;
    }

    private bool CompareValue(int piecesLeft)
    {
        switch(_comparator)
        {
            case Comparator.GREATER_THAN:
                return piecesLeft > _compareValue;
            case Comparator.LESS_THAN:
                return piecesLeft < _compareValue;
            case Comparator.LESS_THAN_OR_EQUALS:
                return piecesLeft <= _compareValue;
            case Comparator.GREATER_THAN_OR_EQUALS:
                return piecesLeft >= _compareValue;
            case Comparator.EQUALS:
                return piecesLeft == _compareValue;
            case Comparator.NOT_EQUALS:
                return piecesLeft != _compareValue;
            default:
                return false;
        }
    }

    public bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        Chessboard board = _state == BoardState.THIS ? thisBoardState : nextBoardState;
        int piecesLeft = Utils.FindPiecesOfType(board, _pieceIdentifier).Count();
        return CompareValue(piecesLeft);
    }

}

public enum Comparator {
    GREATER_THAN, LESS_THAN, GREATER_THAN_OR_EQUALS, LESS_THAN_OR_EQUALS, EQUALS, NOT_EQUALS
}


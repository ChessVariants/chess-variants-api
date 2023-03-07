using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
internal class PieceAt : IPredicate
{
    private readonly string _pieceIdentifier;
    private readonly Tuple<int, int> _position;
    private readonly BoardState _boardState;
    private readonly PositionType _positionType;


    public PieceAt(string pieceIdentifier, Tuple<int, int> position, BoardState boardState, PositionType positionType = PositionType.ABSOLUTE)
    {
        _pieceIdentifier = pieceIdentifier;
        _position = position;
        _boardState = boardState;
        _positionType = positionType;
    }

    public bool Evaluate(BoardTransition transition)
    {
        bool isThisBoardState = _boardState == BoardState.THIS;
        var board = isThisBoardState ? transition.ThisState : transition.NextState;
        var relativePosition = isThisBoardState ? transition.MoveFrom : transition.MoveTo;

        var finalPosition = _position;

        if (_positionType == PositionType.RELATIVE)
            finalPosition = Tuple.Create(_position.Item1 + relativePosition.Item1, _position.Item2 + relativePosition.Item2);

        string? pieceAt = board.Board.GetPieceIdentifier(finalPosition.Item2, finalPosition.Item1);

        return _pieceIdentifier.Equals(pieceAt);

    }
}

public enum PositionType
{
    ABSOLUTE, RELATIVE
}

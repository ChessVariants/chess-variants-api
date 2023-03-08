using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
internal class SquareAttacked : IPredicate
{
    private readonly Tuple<int, int> _position;
    private readonly BoardState _boardState;
    private readonly PositionType _positionType;
    private readonly Player _attacker;


    public SquareAttacked(Tuple<int, int> position, BoardState boardState, Player attacker, PositionType positionType = PositionType.ABSOLUTE)
    {
        _position = position;
        _boardState = boardState;
        _positionType = positionType;
        _attacker = attacker;
    }

    public bool Evaluate(BoardTransition transition)
    {
        bool isThisBoardState = _boardState == BoardState.THIS;
        var board = isThisBoardState ? transition.ThisState : transition.NextState;
        var relativePosition = isThisBoardState ? transition.MoveFrom : transition.MoveTo;

        var finalPosition = _position;

        if (_positionType == PositionType.RELATIVE)
            finalPosition = Tuple.Create(_position.Item1 + relativePosition.Item1, _position.Item2 + relativePosition.Item2);

        board.Board.IndexToCoor.TryGetValue(finalPosition, out var coor);
        if (coor == null) return false;

        return Utils.SquareAttacked(board, coor, _attacker);
    }
}

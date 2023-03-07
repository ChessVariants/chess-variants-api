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
        throw new NotImplementedException();
    }
}

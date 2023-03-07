using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules;
public class BoardTransition
{

    private readonly IBoardState _thisState;
    private readonly IBoardState _nextState;
    private readonly Tuple<int, int> _moveFrom;
    private readonly Tuple<int, int> _moveTo;

    public IBoardState ThisState => _thisState;
    public IBoardState NextState => _nextState;
    public Tuple<int, int> MoveFrom => _moveFrom;
    public Tuple<int, int> MoveTo => _moveTo;

    public BoardTransition(IBoardState thisState, IBoardState nextState, Tuple<int, int> moveFrom, Tuple<int, int> moveTo)
    {
        _thisState = thisState;
        _nextState = nextState;
        _moveFrom = moveFrom;
        _moveTo = moveTo;
    }
    public BoardTransition(IBoardState thisState, IBoardState nextState, string fromTo)
    {
        _thisState = thisState;
        _nextState = nextState;
        (string from, string to) = thisState.parseMove(fromTo);
        var moveFrom = thisState.Board.ParseCoordinate(from);
        var moveTo = thisState.Board.ParseCoordinate(to);

        if (moveFrom == null)
            moveFrom = Tuple.Create(0, 0);
        if (moveTo == null)
            moveTo = Tuple.Create(0, 0);

        _moveFrom = moveFrom;
        _moveTo = moveTo;

    }



}

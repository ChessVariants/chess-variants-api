using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents a board transition from one state to another.
/// The _moveFrom and _moveTo variables should be equal to the position of the piece performing the transition, before and after the transition.
/// </summary>
public class BoardTransition
{

    public readonly MoveWorker _thisState;
    public readonly MoveWorker _nextState;
    public readonly Move _move;
    public readonly GameEvent _result;
    public readonly string _moveFrom;
    public readonly string _moveTo;

    public BoardTransition(MoveWorker thisState, Move move)
    {
        _thisState = thisState;
        _nextState = thisState.CopyBoardState();
        _move = move;
        _result = _move.Perform(_nextState);

        Tuple<string, string>? fromTo = thisState.ParseMove(move.FromTo);
        if (fromTo == null) throw new ArgumentException("The given move parameter does not contain a proper move string. Supplied move string: " + move.FromTo);

        _moveFrom = fromTo.Item1;
        _moveTo = fromTo.Item2;
    }
    
    public bool IsValid()
    {
        return _result != GameEvent.InvalidMove;
    }

}

using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents a board transition from one state to another.
/// The _moveFrom and _moveTo variables should be equal to the position of the piece performing the transition, before and after the transition.
/// </summary>
public class BoardTransition
{

    private readonly MoveWorker _thisState;
    private readonly MoveWorker _nextState;
    private readonly string _moveFrom;
    private readonly string _moveTo;
    private readonly Move _move;

    public MoveWorker ThisState => _thisState;
    public MoveWorker NextState => _nextState;
    public Move Move => _move;
    public string MoveFrom => _moveFrom;
    public string MoveTo => _moveTo;

    public BoardTransition(MoveWorker thisState, MoveWorker nextState, Move move)
    {
        _thisState = thisState;
        _nextState = nextState;
        _move = move;
        Tuple<string, string>? fromTo = thisState.parseMove(move.FromTo);
        if (fromTo == null) throw new ArgumentException("The internal Move does not contain a proper move string");

        _moveFrom = fromTo.Item1;
        _moveTo = fromTo.Item2;
    }
    public BoardTransition(MoveWorker thisState, MoveWorker nextState, string fromTo)
    {
        _thisState = thisState;
        _nextState = nextState;
        Tuple<string, string>? fromToTuple = thisState.parseMove(fromTo);
        if (fromToTuple == null) throw new ArgumentException("The internal Move does not contain a proper move string");
        _move = new MoveStandard(fromTo);

        _moveFrom = fromToTuple.Item1;
        _moveTo = fromToTuple.Item2;
    }


    /*
    public BoardTransition(MoveWorker thisState, MoveWorker nextState, string fromTo)
    {
        _thisState = thisState;
        _nextState = nextState;
        (string from, string to) = thisState.parseMove(fromTo);
        if (from == null || to == null) throw new ArgumentException("Board transition must contain a proper move string");

        _moveFrom = from;
        _moveTo = to;
    }
    public BoardTransition(MoveWorker thisState, MoveWorker nextState, string from, string to)
    {
        _thisState = thisState;
        _nextState = nextState;
        _moveFrom = from;
        _moveTo = to;
    }

    */

}

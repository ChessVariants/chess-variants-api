namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents a board transition from one state to another.
/// The _moveFrom and _moveTo variables should be equal to the position of the piece performing the transition, before and after the transition.
/// </summary>
public class BoardTransition
{

    private readonly MoveWorker _thisState;
    private readonly MoveWorker _nextState;
    private readonly Tuple<int, int> _moveFrom;
    private readonly Tuple<int, int> _moveTo;

    public MoveWorker ThisState => _thisState;
    public MoveWorker NextState => _nextState;
    public Tuple<int, int> MoveFrom => _moveFrom;
    public Tuple<int, int> MoveTo => _moveTo;

    public BoardTransition(MoveWorker thisState, MoveWorker nextState, Tuple<int, int> moveFrom, Tuple<int, int> moveTo)
    {
        _thisState = thisState;
        _nextState = nextState;
        _moveFrom = moveFrom;
        _moveTo = moveTo;
    }
    public BoardTransition(MoveWorker thisState, MoveWorker nextState, string fromTo)
    {
        _thisState = thisState;
        _nextState = nextState;
        Tuple<string, string>? fromToStr = thisState.parseMove(fromTo);
        if(fromToStr == null)
        {
            throw new ArgumentException("Board Transition must contain fromTo move information (e.g. f1f2");
        }
        (string from, string to) = fromToStr;
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

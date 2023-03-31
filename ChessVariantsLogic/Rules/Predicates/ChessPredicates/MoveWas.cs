namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// This predicate evaluates if the last move is equal to the compare values.
/// </summary>
public class MoveWas : IPredicate
{
    private readonly IPosition _compareFrom;
    private readonly IPosition _compareTo;
    private readonly MoveState _moveState;

    public MoveWas(IPosition compareFrom, IPosition compareTo, MoveState moveState)
    {
        _compareFrom = compareFrom;
        _compareTo = compareTo;
        _moveState = moveState;
    }

    public bool Evaluate(BoardTransition transition)
    {
        Move? move = null;
        if (_moveState == MoveState.THIS)
            move = transition.Move;
        else if(_moveState == MoveState.LAST)
            move = transition.ThisState.GetLastMove();
        if (move == null) return false;

        Tuple<string, string>? lastMove = MoveWorker.ParseMove(move.FromTo);
        if(lastMove == null) return false;
        string? from = _compareFrom.GetPosition(transition.ThisState, transition.MoveFrom);
        string? to   = _compareTo.GetPosition(transition.ThisState, transition.MoveFrom);

        if (from == null) return to == lastMove.Item2;
        if (to == null) return from == lastMove.Item1;

        string compareMove = from + to;
        return compareMove == (lastMove.Item1 + lastMove.Item2);
    }
}

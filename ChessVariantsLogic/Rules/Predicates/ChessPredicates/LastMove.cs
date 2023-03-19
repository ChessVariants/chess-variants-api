namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// This predicate evaluates if the last move is equal to the compare values.
/// </summary>
public class LastMove : IPredicate
{
    private readonly IPosition _compareFrom;
    private readonly IPosition _compareTo;

    public LastMove(IPosition compareFrom, IPosition compareTo)
    {
        _compareFrom = compareFrom;
        _compareTo = compareTo;
    }

    public bool Evaluate(BoardTransition transition)
    {
        Move? move = transition.ThisState.getLastMove();
        if (move == null) return false;
        string lastMove = move.FromTo;
        string? from = _compareFrom.GetPosition(transition.ThisState, transition.MoveFrom);
        string? to   = _compareTo.GetPosition(transition.ThisState, transition.MoveFrom);

        if (from == null || to == null) return false;
        string compareMove = from + to;
        return compareMove == lastMove;
    }
}

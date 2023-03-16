namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

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
        string? lastMove = transition._thisState.getLastMove();
        string? from = _compareFrom.GetPosition(transition._thisState, transition._moveFrom);
        string? to   = _compareTo.GetPosition(transition._thisState, transition._moveFrom);

        if (lastMove == null || from == null || to == null) return false;
        string compareMove = from + to;
        return compareMove == lastMove;
    }
}

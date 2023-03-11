namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate evaluates if a piece at a given location has moved.
/// </summary>
public class HasMoved : IPredicate
{
    private readonly IPosition _position;


    public HasMoved(IPosition position)
    {
        _position = position;
    }

    public bool Evaluate(BoardTransition transition)
    {
        string? coordinate = _position.GetPosition(transition.ThisState, transition.MoveFrom);
        if (coordinate == null) return false;
        Tuple<int, int>? tupleCoordinate = transition.ThisState.Board.ParseCoordinate(coordinate);
        if (tupleCoordinate == null) return false;
        return transition.ThisState.Board.HasPieceMoved(tupleCoordinate.Item1, tupleCoordinate.Item2);
    }
}

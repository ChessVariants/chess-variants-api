namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate evaluates if a piece at a given location has moved.
/// </summary>
public class HasMoved : IPredicate
{
    private readonly IPosition _position;
    private readonly BoardState _boardState;
    private readonly RelativeTo _relativeTo;

    public HasMoved(IPosition position, BoardState boardState = BoardState.THIS, RelativeTo relativeTo = RelativeTo.FROM)
    {
        _position = position;
        _boardState = boardState;
        _relativeTo = relativeTo;
    }

    public bool Evaluate(BoardTransition transition)
    {
        MoveWorker boardState = _boardState == BoardState.THIS ? transition.ThisState : transition.NextState;
        string relativePosition = _relativeTo == RelativeTo.FROM ? transition.MoveFrom : transition.MoveTo;

        string? coordinate = _position.GetPosition(boardState, relativePosition);
        if (coordinate == null) return false;
        Tuple<int, int>? tupleCoordinate = boardState.Board.ParseCoordinate(coordinate);
        if (tupleCoordinate == null) return false;
        return boardState.Board.HasPieceMoved(tupleCoordinate.Item1, tupleCoordinate.Item2);
    }
}

using ChessVariantsLogic.Rules.Predicates.ChessPredicates.NewPredicates;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate evaluates if a piece at a given location has moved.
/// </summary>
public class HasMoved : SquarePredicate
{
    
    public HasMoved(IPosition position, BoardState boardState = BoardState.THIS, RelativeTo relativeTo = RelativeTo.FROM) : base(boardState, relativeTo, position)
    {
    }

    public override bool Evaluate(BoardTransition transition)
    {
        MoveWorker boardState = GetBoardState(transition);
        string? coordinate = GetFinalPosition(transition);

        if (coordinate == null) return false;
        Tuple<int, int>? tupleCoordinate = boardState.Board.ParseCoordinate(coordinate);
        if (tupleCoordinate == null) return false;
        return boardState.Board.HasPieceMoved(tupleCoordinate.Item1, tupleCoordinate.Item2);
    }
}

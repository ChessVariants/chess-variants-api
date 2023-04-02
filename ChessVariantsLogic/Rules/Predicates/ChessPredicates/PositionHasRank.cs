
namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square has a certain rank.
/// The target position can be calculated relatively or absolutely.
/// </summary>
public class PositionHasRank : IPredicate
{
    private readonly IPosition _position;
    private readonly int _rank;
    private readonly RelativeTo _relativeTo;


    public PositionHasRank(IPosition position, int rank, RelativeTo relativeTo = RelativeTo.FROM)
    {
        _position = position;
        _rank = rank;
        _relativeTo = relativeTo;
    }
    /// <summary>
    /// Evaluates to true/false if a square has a certain rank.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a square has a certain rank.</returns>

    public bool Evaluate(BoardTransition boardTransition)
    {
        var pivotPosition = _relativeTo == RelativeTo.FROM ? boardTransition.MoveFrom : boardTransition.MoveTo;
        var board = boardTransition.ThisState;

        Tuple<int, int>? finalPosition = _position.GetPositionTuple(board, pivotPosition);
        if (finalPosition == null) return false;

        return _rank == (board.Board.Rows - finalPosition.Item1);
    }   
}


/// <summary>
/// An enum that determines whether predicates that evaluate positions should calculate their positions relative to the From or To variables of the supplied BoardTransition.
/// </summary>
public enum RelativeTo
{
    FROM, TO
}

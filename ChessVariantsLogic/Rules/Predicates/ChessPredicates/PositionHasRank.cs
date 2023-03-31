
using ChessVariantsLogic.Rules.Predicates.ChessPredicates.NewPredicates;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square has a certain rank.
/// The target position can be calculated relatively or absolutely.
/// </summary>
public class PositionHasRank : SquarePredicate
{
    private readonly int _rank;


    public PositionHasRank(IPosition position, int rank, RelativeTo relativeTo = RelativeTo.FROM) : base(BoardState.THIS, relativeTo, position)
    {
        _rank = rank;
    }
    /// <summary>
    /// Evaluates to true/false if a square has a certain rank.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a square has a certain rank.</returns>

    public override bool Evaluate(BoardTransition boardTransition)
    {
        var pivotPosition = GetRelativeTo(boardTransition);
        var board = boardTransition.ThisState;

        Tuple<int, int>? finalPosition = _square.GetPositionTuple(board, pivotPosition);
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

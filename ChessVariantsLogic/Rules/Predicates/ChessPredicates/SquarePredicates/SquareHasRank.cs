
using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square has a certain rank.
/// The target position can be calculated relatively or absolutely.
/// </summary>
public class SquareHasRank : SquarePredicate
{
    [JsonProperty]
    private readonly int _rank;


    public SquareHasRank(IPosition position, int rank) : base(BoardState.THIS, position)
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
        var board = boardTransition.ThisState;

        Tuple<int, int>? finalPosition = _square.GetPositionTuple(board, boardTransition.MoveFromTo);
        if (finalPosition == null) return false;

        return _rank == (board.Board.Rows - finalPosition.Item1);
    }   
}


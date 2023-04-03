
namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square has a certain file.
/// The target position can be calculated relatively or absolutely.
/// </summary>
public class SquareHasFile : SquarePredicate
{
    private readonly int _file;


    public SquareHasFile(IPosition position, int file) : base(BoardState.THIS, position)
    {
        _file = file;
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

        return _file == (board.Board.Cols - finalPosition.Item2);
    }   
}


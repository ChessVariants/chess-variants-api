
namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square has the same coordinates as another square.
/// </summary>
public class SquareIs : SquarePredicate
{
    private readonly IPosition _isPosition;


    public SquareIs(IPosition position, IPosition isPosition) : base(BoardState.THIS, position)
    {
        _isPosition = isPosition;
    }
    /// <summary>
    /// Evaluates to true/false if a square has a certain rank.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a square has a certain rank.</returns>

    public override bool Evaluate(BoardTransition boardTransition)
    {
        var board = GetBoardState(boardTransition);

        return _isPosition.GetPosition(board, boardTransition.MoveFromTo) == GetFinalPosition(boardTransition);
    }   
}


namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square is attacked or not by the given player, either in the current board state or the next.
/// The target position can be calculated relatively or absolutely.
/// </summary>
public class SquareAttacked : SquarePredicate
{
    private readonly Player _attacker;


    public SquareAttacked(IPosition position, BoardState boardState, Player attacker) : base(boardState, position)
    {
        _attacker = attacker;
    }
    /// <summary>
    /// Evaluates to true/false if a square is attacked or not in either the current board state or the next, depending on what was specified at object-creation.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a square is attacked or not in either the current board state or the next, depending on what was specified at object-creation.</returns>

    public override bool Evaluate(BoardTransition boardTransition)
    {
        var board = GetBoardState(boardTransition);
        string? finalPosition = GetFinalPosition(boardTransition);
        
        if (finalPosition == null) return false;

        return Utils.SquareAttacked(board, finalPosition, _attacker);
    }
}

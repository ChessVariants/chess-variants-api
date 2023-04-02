
namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square is attacked or not by the given player, either in the current board state or the next.
/// The target position can be calculated relatively or absolutely.
/// </summary>
public class SquareAttacked : IPredicate
{
    private readonly IPosition _position;
    private readonly BoardState _boardState;
    private readonly Player _attacker;
    private readonly RelativeTo _relativeTo;


    public SquareAttacked(IPosition position, BoardState boardState, Player attacker, RelativeTo relativeTo = RelativeTo.FROM)
    {
        _position = position;
        _boardState = boardState;
        _attacker = attacker;
        _relativeTo = relativeTo;
    }
    /// <summary>
    /// Evaluates to true/false if a square is attacked or not in either the current board state or the next, depending on what was specified at object-creation.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a square is attacked or not in either the current board state or the next, depending on what was specified at object-creation.</returns>

    public bool Evaluate(BoardTransition boardTransition)
    {
        bool isThisBoardState = _boardState == BoardState.THIS;
        var board = isThisBoardState ? boardTransition.ThisState : boardTransition.NextState;
        string relativePosition = _relativeTo == RelativeTo.FROM ? boardTransition.MoveFrom : boardTransition.MoveTo;

        string? finalPosition = _position.GetPosition(board, relativePosition);
        if (finalPosition == null) return false;

        return Utils.SquareAttacked(board, finalPosition, _attacker);
    }   
}

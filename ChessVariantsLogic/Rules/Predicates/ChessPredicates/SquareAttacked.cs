
namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate determines if a square is attacked or not by the given player, either in the current board state or the next.
/// The target position can be calculated relatively or absolutely.
/// </summary>
public class SquareAttacked : IPredicate
{
    private readonly Tuple<int, int> _position;
    private readonly BoardState _boardState;
    private readonly PositionType _positionType;
    private readonly Player _attacker;


    public SquareAttacked(Tuple<int, int> position, BoardState boardState, Player attacker, PositionType positionType = PositionType.ABSOLUTE)
    {
        _position = position;
        _boardState = boardState;
        _positionType = positionType;
        _attacker = attacker;
    }
    /// <summary>
    /// Evaluates to true/false if a square is attacked or not in either the current board state or the next, depending on what was specified at object-creation.
    /// If _positionType is PositionType.RELATIVE, the target position will be calculated relative to the piece being moved in the boardTransition
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a square is attacked or not in either the current board state or the next, depending on what was specified at object-creation.</returns>

    public bool Evaluate(BoardTransition boardTransition)
    {
        bool isThisBoardState = _boardState == BoardState.THIS;
        var board = isThisBoardState ? boardTransition.ThisState : boardTransition.NextState;
        var relativePosition = boardTransition.MoveFrom;

        var finalPosition = _position;

        if (_positionType == PositionType.RELATIVE)
            finalPosition = Tuple.Create(_position.Item1 + relativePosition.Item1, _position.Item2 + relativePosition.Item2);

        board.Board.IndexToCoor.TryGetValue(finalPosition, out var coor);
        if (coor == null) return false;

        return Utils.SquareAttacked(board, coor, _attacker);
    }
}

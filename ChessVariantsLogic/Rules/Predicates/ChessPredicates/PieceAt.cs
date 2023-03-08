namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate evaluates if a given board coordinate contains a piece of the internal _pieceIdentifier.
/// </summary>
public class PieceAt : IPredicate
{
    private readonly string _pieceIdentifier;
    private readonly Tuple<int, int> _position;
    private readonly BoardState _boardState;
    private readonly PositionType _positionType;


    public PieceAt(string pieceIdentifier, Tuple<int, int> position, BoardState boardState, PositionType positionType = PositionType.ABSOLUTE)
    {
        _pieceIdentifier = pieceIdentifier;
        _position = position;
        _boardState = boardState;
        _positionType = positionType;
    }

    /// <summary>
    /// Evaluates to true/false if the identifier of the piece at the internal _position is equal to the internal _pieceIdentifier.
    /// If _positionType is PositionType.RELATIVE, the target position will be calculated relative to the piece being moved (retrieved from the boardTransition object).
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if the identifier of the piece at the internal _position is equal to the internal _pieceIdentifier.</returns>

    public bool Evaluate(BoardTransition transition)
    {
        bool isThisBoardState = _boardState == BoardState.THIS;
        var board = isThisBoardState ? transition.ThisState : transition.NextState;
        var relativePosition = transition.MoveFrom;

        var finalPosition = _position;

        if (_positionType == PositionType.RELATIVE)
            finalPosition = Tuple.Create(_position.Item1 + relativePosition.Item1, _position.Item2 + relativePosition.Item2);

        string? pieceAt = board.Board.GetPieceIdentifier(finalPosition.Item1, finalPosition.Item2);

        return _pieceIdentifier.Equals(pieceAt);

    }
}
/// <summary>
/// Enums for the two types of positions that can be used for predicates that evaluate positions.
/// Absolute positions are specified at object creation
/// Relative positions are calculated from the given BoardTransition object of the Evaluate method.
/// </summary>

public enum PositionType
{
    ABSOLUTE, RELATIVE
}

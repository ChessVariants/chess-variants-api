namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate evaluates if a given board coordinate contains a piece that has the type of the internal _pieceIdentifier.
/// </summary>
public class PieceAt : SquarePredicate
{
    private readonly string _pieceIdentifier;


    public PieceAt(string pieceIdentifier, IPosition position, BoardState boardState) : base(boardState, position)
    {
        _pieceIdentifier = pieceIdentifier;
    }

    /// <summary>
    /// Evaluates to true/false if the identifier of the piece at the internal _position is equal to the internal _pieceIdentifier.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if the identifier of the piece at the internal _position is equal to the internal _pieceIdentifier.</returns>

    public override bool Evaluate(BoardTransition transition)
    {
        MoveWorker board = GetBoardState(transition);
        string? finalPosition = GetFinalPosition(transition);

        if (finalPosition == null) return false;

        string? pieceAt = board.Board.GetPieceIdentifier(finalPosition);

        return _pieceIdentifier.Equals(pieceAt);
    }
}

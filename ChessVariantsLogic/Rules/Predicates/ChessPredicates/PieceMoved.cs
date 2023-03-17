namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate determines if a piece has been captured when transitioning to a new board state.
/// </summary>
public class PieceMoved : IPredicate
{
    private readonly string _pieceIdentifier;

    public PieceMoved(string pieceIdentifier)
    {
        _pieceIdentifier = pieceIdentifier;
    }

    /// <summary>
    /// Returns true if a piece was captured when transitioning from <paramref name="thisBoardState"/> to <paramref name="nextBoardState"/>.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>True if a piece was captured during transition from <paramref name="thisBoardState"/> to <paramref name="nextBoardState"/>.</returns>
    public bool Evaluate(BoardTransition transition)
    {
        string piece = transition.ThisState.Board.GetPieceIdentifier(transition.MoveFrom);

        return (piece == _pieceIdentifier) && (transition.MoveFrom != transition.MoveTo);
    }

}
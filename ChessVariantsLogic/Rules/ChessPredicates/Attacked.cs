namespace ChessVariantsLogic.Predicates;

/// <summary>
/// This predicate determines if a piece is attacked or not, either in the current board state or the next.
/// </summary>
public class Attacked : IPredicate
{
    private readonly BoardState _boardState;
    private readonly string _pieceIdentifier;

    public Attacked(BoardState boardState, string pieceIdentifier)
    {
        _boardState = boardState;
        _pieceIdentifier = pieceIdentifier;
    }

    /// <summary>
    /// Evaluates to true/false if a piece is attacked or not in either the current board state or the next, depending on what was specified at object-creation.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a piece is attacked or not in either the current board state or the next, depending on what was specified at object-creation.</returns>
    public bool Evaluate(IBoardState thisBoardState, IBoardState nextBoardState)
    {
        IBoardState board = _boardState == BoardState.NEXT ? nextBoardState : thisBoardState;
        var attacked = Utils.PieceAttacked(board, _pieceIdentifier);
        return attacked;
    }

}
public enum BoardState
{
    THIS, NEXT
}
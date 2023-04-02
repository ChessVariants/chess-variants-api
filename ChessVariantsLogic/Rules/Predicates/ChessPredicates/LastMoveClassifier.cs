namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// This predicate evaluates if the piece performing the last move has the same classifier as the internal _pieceClassifier.
/// </summary>
public class LastMoveClassifier : IPredicate
{
    private readonly PieceClassifier _pieceClassifier;

    public LastMoveClassifier(PieceClassifier pieceClassifier)
    {
        _pieceClassifier = pieceClassifier;
    }

    public bool Evaluate(BoardTransition transition)
    {
        Move? move = transition.ThisState.GetLastMove();
        if (move == null) return false;
        PieceClassifier lastMovePlayer = move.PieceClassifier;
        return _pieceClassifier == lastMovePlayer;
    }
}

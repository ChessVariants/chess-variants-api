namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// This predicate evaluates if the last move is equal to the compare values.
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
        Move? move = transition.ThisState.getLastMove();
        if (move == null) return false;
        PieceClassifier lastMovePlayer = move.Player;
        return _pieceClassifier == lastMovePlayer;
    }
}

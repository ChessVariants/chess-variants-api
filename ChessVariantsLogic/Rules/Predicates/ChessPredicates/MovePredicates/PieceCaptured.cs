using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate determines if a piece has been captured when transitioning to a new board state.
/// </summary>
public class PieceCaptured : MovePredicate
{
    private readonly string _pieceIdentifier;

    public PieceCaptured(string pieceIdentifier, MoveState moveState = MoveState.THIS) : base(moveState) 
    {
        _pieceIdentifier = pieceIdentifier;
    }

    /// <summary>
    /// Returns true if a piece was captured during the given <paramref name="transition"/>.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>True if a piece was captured during the given <paramref name="transition"/></returns>
    public override bool Evaluate(BoardTransition transition)
    {
        Move? move = GetMove(transition);
        if (move == null)
            return false;
        return move.CapturedPiece(_pieceIdentifier);
    }
}

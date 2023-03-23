using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules.Moves.Actions;

/// <summary>
/// When performed this action will place the internal _pieceIdentifier at the target position.
/// </summary>
public class ActionSetPiece : Action
{
    private readonly IPosition _at;
    private readonly string _pieceIdentifier;

    public ActionSetPiece(IPosition at, string pieceIdentifier, RelativeTo relativeTo) : base(relativeTo)
    {
        _at = at;
        _pieceIdentifier = pieceIdentifier;
    }

    /// <summary>
    /// Inserts a piece with the internal _pieceIdentifier on the board according to the internal _at position.
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="pivotPosition">This position is used to calculate the final position using the GetPosition method.</param>
    /// 
    /// <returns>A GameEvent that represents whether or not the action was successfully performed.</returns>
    /// 
    protected override GameEvent Perform(MoveWorker moveWorker, string pivotPosition)
    {
        string? finalPosition = _at.GetPosition(moveWorker, pivotPosition);
        if (finalPosition == null) return GameEvent.InvalidMove;
        bool performedSucessfully = moveWorker.Board.Insert(_pieceIdentifier, finalPosition);
        if (performedSucessfully)
            return GameEvent.MoveSucceeded;
        else
            return GameEvent.InvalidMove;
    }
}

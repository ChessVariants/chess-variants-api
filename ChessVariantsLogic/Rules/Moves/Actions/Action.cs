global using Action = ChessVariantsLogic.Rules.Moves.Actions.Action;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules.Moves.Actions;

/// <summary>
/// Represents an action that can be performed on the board.
/// </summary>
public abstract class Action
{
    protected Piece? _capturedPiece = null;

    public bool DidCapturePiece(string pieceIdentifier)
    {
        return Utils.IsOfType(_capturedPiece, pieceIdentifier);
    }

    public void ClearAction()
    {
        _capturedPiece = null;
    }

    /// <summary>
    /// Performs an action on the given moveWorker according to implementation.
    /// This method should only be called from the MoveWorker class
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="moveCoordinates">The move coordinates that any PositionRelative will use to calculate it's position relative to.</param>
    /// 
    /// <returns>A GameEvent that occured when the action was performed.</returns>
    /// 
    public abstract GameEvent Perform(MoveWorker moveWorker, string moveCoordinates);

}
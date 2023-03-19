namespace ChessVariantsLogic.Rules.Moves.Actions;

internal class ActionSetPiece : IAction
{
    private readonly IPosition _at;
    private readonly string _pieceIdentifier;

    public ActionSetPiece(IPosition at, string pieceIdentifier)
    {
        _at = at;
        _pieceIdentifier = pieceIdentifier;
    }


    /// <summary>
    /// Delete a piece on the board according to the internal _at position.
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">This position is used to calculate the final position using the GetPosition method.</param>
    /// 
    /// <returns>A GameEvent that represents whether or not the action was successfully performed.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker, string performingPiecePosition)
    {
        string? finalPosition = _at.GetPosition(moveWorker, performingPiecePosition);
        if (finalPosition == null) return GameEvent.InvalidMove;
        bool performedSucessfully = moveWorker.Board.Insert(_pieceIdentifier, finalPosition);
        if (performedSucessfully)
            return GameEvent.MoveSucceeded;
        else
            return GameEvent.InvalidMove;
    }
}

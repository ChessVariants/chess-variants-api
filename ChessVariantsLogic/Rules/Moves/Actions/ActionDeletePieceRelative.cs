namespace ChessVariantsLogic.Rules.Moves.Actions;

/// <summary>
/// When performed this action will delete a piece on the board according to the given relative positions.
/// </summary>

public class ActionDeletePieceRelative : IAction
{
    private readonly Tuple<int, int> _at;

    public ActionDeletePieceRelative(Tuple<int, int> at)
    {
        _at = at;
    }

    /// <summary>
    /// Delete a piece on the board at the internal _at coordinate relative to the given piecePosition.
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">The position of the piece performing the action.</param>
    /// 
    /// <returns>A GameEvent that represents whether or not the action was successfully performed.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker, string performingPiecePosition)
    {
        string? atAbsolute = GetAtAbsolute(moveWorker, performingPiecePosition);
        if (atAbsolute == null) return GameEvent.InvalidMove;

        if (moveWorker.Board.Insert(Constants.UnoccupiedSquareIdentifier, atAbsolute))
            return GameEvent.MoveSucceeded;
        else
            return GameEvent.InvalidMove;
    }

    /// <summary>
    /// Calculate the absolute position of the piece being deleted.
    /// The absolute at position is calculated as the internal _at position relative to performingPiecePosition.
    /// 
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">The position of the piece performing the action.</param>
    /// 
    /// <returns>The final absolute position of the piece being deleted in string format. (E.g. "e2")</returns>
    /// 
    private string? GetAtAbsolute(MoveWorker moveWorker, string performingPiecePosition)
    {
        Tuple<int, int>? performingPiecePositionTuple = moveWorker.Board.ParseCoordinate(performingPiecePosition);
        if (performingPiecePositionTuple == null)
            return null;
        string? atAbsolute;

        Tuple<int, int> atPosAbsolute = Tuple.Create(performingPiecePositionTuple.Item1 + _at.Item1, performingPiecePositionTuple.Item2 + _at.Item2);

        moveWorker.Board.IndexToCoor.TryGetValue(atPosAbsolute, out atAbsolute);

        return atAbsolute;
    }
}

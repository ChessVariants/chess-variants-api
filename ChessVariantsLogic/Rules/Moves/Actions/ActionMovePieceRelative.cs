namespace ChessVariantsLogic.Rules.Moves.Actions;

/// <summary>
/// When performed this action will forcefully move a piece on the board according to the given relative positions.
/// </summary>
public class ActionMovePieceRelative : IAction
{
    private readonly Tuple<int, int> _from;
    private readonly Tuple<int, int> _to;

    public ActionMovePieceRelative(Tuple<int, int> from, Tuple<int, int> to)
    {
        _from = from;
        _to = to;
    }

    public ActionMovePieceRelative(Tuple<int, int> to)
    {
        _from = Tuple.Create(0, 0);
        _to = to;
    }

    /// <summary>
    /// Forcefully move a piece on the board according to the internal variables relative to the given piecePosition.
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">The position of the piece performing the action.</param>
    /// 
    /// <returns>A GameEvent that represents whether or not the action was successfully performed.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker, string performingPiecePosition)
    {
        string? fromTo = GetFromToAbsolute(moveWorker, performingPiecePosition);
        if (fromTo == null) return GameEvent.InvalidMove;
        return moveWorker.ForceMove(fromTo);
    }

    /// <summary>
    /// Calculate the from and to positions for the move action.
    /// The finalFrom is calculated as the internal _from position relative to the piece performing the action.
    /// The finalTo is calculated as the internal _to position relative to the finalFrom position.
    /// 
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">The position of the piece performing the action.</param>
    /// 
    /// <returns>The final absolute positions of the move in string format. (E.g. "e2e4")</returns>
    /// 
    private string? GetFromToAbsolute(MoveWorker moveWorker, string performingPiecePosition)
    {
        string? finalFrom;
        string? finalTo;
        Tuple<int, int>? moveFromPos = moveWorker.Board.ParseCoordinate(performingPiecePosition);
        if (moveFromPos == null) return null;

        Tuple<int, int> actionFromPos = Tuple.Create(moveFromPos.Item1 + _from.Item1, moveFromPos.Item2 + _from.Item2);
        Tuple<int, int> actionToPos = Tuple.Create(actionFromPos.Item1 + _to.Item1, actionFromPos.Item2 + _to.Item2);

        moveWorker.Board.IndexToCoor.TryGetValue(actionFromPos, out finalFrom);
        moveWorker.Board.IndexToCoor.TryGetValue(actionToPos, out finalTo);

        return finalFrom + finalTo;
    }

}

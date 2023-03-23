using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules.Moves.Actions;

/// <summary>
/// Represents an action that can be performed on the board.
/// </summary>
public abstract class Action
{
    private readonly RelativeTo _relativeTo;

    public Action(RelativeTo relativeTo)
    {
        _relativeTo = relativeTo;
    }

    public Action()
    {
        _relativeTo = RelativeTo.FROM;
    }


    /// <summary>
    /// Performs an action on the given moveWorker according to implementation.
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">The position of the piece that performed the action. Relative positions are calculated relative to this position.</param>
    /// 
    /// <returns>A GameEvent that represents whether or not the action was successfully performed.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker, string from, string to)
    {
        string pivotPosition = GetPivotPosition(from, to);
        return Perform(moveWorker, pivotPosition);
    }

    protected abstract GameEvent Perform(MoveWorker moveWorker, string pivotPosition);

    private string GetPivotPosition(string from, string to)
    {
        return _relativeTo == RelativeTo.FROM ? from : to;
    }

}
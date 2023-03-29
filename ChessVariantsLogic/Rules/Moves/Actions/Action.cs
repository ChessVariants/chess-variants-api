global using Action = ChessVariantsLogic.Rules.Moves.Actions.Action;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using System;


namespace ChessVariantsLogic.Rules.Moves.Actions;

/// <summary>
/// Represents an action that can be performed on the board.
/// </summary>
public abstract class Action
{
    private readonly RelativeTo _relativeTo;

    protected Action(RelativeTo relativeTo)
    {
        _relativeTo = relativeTo;
    }


    /// <summary>
    /// Performs an action on the given moveWorker according to implementation.
    /// This method should only be called from the MoveWorker class
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="from">The position of the piece that performed the action before the action was performed.</param>
    /// <param name="to">The position of the piece that performed the action after the action was performed.</param>
    /// 
    /// <returns>A GameEvent that occured when the action was performed.</returns>
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
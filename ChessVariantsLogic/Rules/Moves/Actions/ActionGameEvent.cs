using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules.Moves.Actions;

/// <summary>
/// This is an abstract class used to create actions which simply return a GameEvent and don't actually change the board.
/// </summary>
public abstract class ActionGameEvent : Action
{
    private readonly GameEvent _eventType;

    protected ActionGameEvent(GameEvent eventType)
    {
        _eventType = eventType;
    }

    public override GameEvent Perform(MoveWorker moveWorker, string moveCoordinates)
    {
        return _eventType;
    }
}

using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules.Moves.Actions;
public abstract class ActionGameEvent : Action
{
    private readonly GameEvent _eventType;

    protected ActionGameEvent(GameEvent eventType) : base(RelativeTo.FROM)
    {
        _eventType = eventType;
    }

    protected override GameEvent Perform(MoveWorker moveWorker, string pivotPosition)
    {
        return _eventType;
    }
}

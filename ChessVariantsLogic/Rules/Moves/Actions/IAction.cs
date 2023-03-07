namespace ChessVariantsLogic.Rules.Moves.Actions;

using Predicates;

public interface IAction
{
    public GameEvent Perform(IBoardState moveWorker, string from);
}
namespace ChessVariantsLogic.Actions;

using Predicates;

public interface IAction
{
    public void Perform(IBoardState moveWorker);
}
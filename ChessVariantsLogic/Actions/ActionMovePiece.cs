namespace ChessVariantsLogic.Actions;

using Predicates;

public class ActionMovePiece : IAction
{
    private string _fromTo;

    public ActionMovePiece(string fromTo)
    {
        _fromTo = fromTo;
    }

    public void Perform(IBoardState moveWorker)
    {
        moveWorker.Move(_fromTo);
    }
}
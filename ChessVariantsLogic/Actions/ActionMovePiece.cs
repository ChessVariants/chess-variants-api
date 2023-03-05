namespace ChessVariantsLogic.Actions;

using Predicates;

public class ActionMovePiece : IAction
{
    private IMoveType _moveType;

    public ActionMovePiece(IMoveType moveType)
    {
        _moveType = moveType;
    }

    public void Perform(IBoardState moveWorker)
    {
        moveWorker.Move(_moveType.GetFromTo(moveWorker));
    }
}

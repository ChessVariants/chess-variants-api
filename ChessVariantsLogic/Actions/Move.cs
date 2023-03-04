namespace ChessVariantsLogic.Actions;

using Predicates;

public class Move
{
    private IEnumerable<IAction> _actions;
    private IPredicate _predicate;
    private string _pieceIdentifier;

    public Move(IEnumerable<IAction> actions, IPredicate predicate, string pieceIdentifier)
    {
        _actions = actions;
        _predicate = predicate;
        _pieceIdentifier = pieceIdentifier;
    }
    public IEnumerable<IAction> Actions  => _actions;
    public bool EvaluatePredicate(IBoardState thisBoardState, IBoardState nextBoardState)
    {
        return _predicate.Evaluate(thisBoardState, nextBoardState);
    }
    public string PieceIdentifier => _pieceIdentifier;
    public void Perform(IBoardState moveWorker)
    {
        foreach(IAction action in _actions)
        {
            action.Perform(moveWorker);
        }
    }

}
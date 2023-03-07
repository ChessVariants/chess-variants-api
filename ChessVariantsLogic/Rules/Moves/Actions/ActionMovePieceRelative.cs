namespace ChessVariantsLogic.Rules.Moves.Actions;

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

    public GameEvent Perform(IBoardState moveWorker, string from)
    {
        string? fromTo = GetFromTo(moveWorker, moveWorker.Board.ParseCoordinate(from));
        if (fromTo == null) return GameEvent.InvalidMove;
        return moveWorker.Move(fromTo);
    }

    private string? GetFromTo(IBoardState moveWorker, Tuple<int, int>? moveFromPos)
    {
        string? from;
        string? to;
        if (moveFromPos == null) return null;

        Tuple<int, int> actionFromPos = Tuple.Create(moveFromPos.Item1 + _from.Item1, moveFromPos.Item2 + _from.Item2);
        Tuple<int, int> actionToPos = Tuple.Create(actionFromPos.Item1 + _to.Item1, actionFromPos.Item2 + _to.Item2);

        moveWorker.Board.IndexToCoor.TryGetValue(actionFromPos, out from);
        moveWorker.Board.IndexToCoor.TryGetValue(actionToPos, out to);

        return from + to;
    }

}

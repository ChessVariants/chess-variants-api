using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Moves.Actions;
public class ActionDeletePieceRelative : IAction
{
    private readonly Tuple<int, int> _atRelative;

    public ActionDeletePieceRelative(Tuple<int, int> at)
    {
        _atRelative = at;
    }

    public GameEvent Perform(IBoardState moveWorker, string from)
    {
        Tuple<int, int>? fromPos = moveWorker.Board.ParseCoordinate(from);
        if (fromPos == null) return GameEvent.InvalidMove;

        string? atAbsolute = GetAtAbsolute(moveWorker, fromPos);
        if (atAbsolute == null) return GameEvent.InvalidMove;

        if (moveWorker.Board.Insert(Constants.UnoccupiedSquareIdentifier, atAbsolute))
            return GameEvent.MoveSucceeded;
        else
            return GameEvent.InvalidMove;
    }

    private string? GetAtAbsolute(IBoardState moveWorker, Tuple<int, int> moveFromPos)
    {
        string? atAbsolute;

        Tuple<int, int> atPosAbsolute = Tuple.Create(moveFromPos.Item1 + _atRelative.Item1, moveFromPos.Item2 + _atRelative.Item2);

        moveWorker.Board.IndexToCoor.TryGetValue(atPosAbsolute, out atAbsolute);

        return atAbsolute;
    }
}

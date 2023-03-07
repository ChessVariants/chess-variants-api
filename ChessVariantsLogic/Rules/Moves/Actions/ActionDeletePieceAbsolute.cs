using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Moves.Actions;
public class ActionDeletePieceAbsolute : IAction
{
    private readonly string _at;

    public ActionDeletePieceAbsolute(string at)
    {
        _at = at;
    }

    public GameEvent Perform(IBoardState moveWorker, string from)
    {
        if (moveWorker.Board.Insert(Constants.UnoccupiedSquareIdentifier, _at))
            return GameEvent.MoveSucceeded;
        else
            return GameEvent.InvalidMove;
    }
}

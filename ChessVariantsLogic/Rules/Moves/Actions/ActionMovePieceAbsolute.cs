using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Moves.Actions;
public class ActionMovePieceAbsolute : IAction
{
    private readonly string _fromTo;

    public ActionMovePieceAbsolute(string fromTo)
    {
        _fromTo = fromTo;
    }

    public GameEvent Perform(IBoardState moveWorker, string from)
    {
        return moveWorker.Move(_fromTo);
    }
}

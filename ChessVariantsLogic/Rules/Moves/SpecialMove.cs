using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;
public class SpecialMove
{
    private readonly IEnumerable<IAction> _actions;
    private readonly string _fromTo;

    public string FromTo => _fromTo;

    public SpecialMove(IEnumerable<IAction> actions, string fromTo)
    {
        _actions = actions;
        _fromTo = fromTo;
    }

    public GameEvent Perform(IBoardState moveWorker)
    {
        (string from, string _) = moveWorker.parseMove(_fromTo);
        foreach (var action in _actions)
        {
            GameEvent gameEvent = action.Perform(moveWorker, from);
            if(gameEvent == GameEvent.InvalidMove)
                return GameEvent.InvalidMove;
        }
        return GameEvent.MoveSucceeded;
    }

}

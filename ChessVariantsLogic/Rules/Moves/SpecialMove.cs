using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;
/// <summary>
/// Represents a special move that can be performed on the board. (examples: castling, en passant, double pawn move...)
/// _actions is a list of actions that will be performed when the SpecialMove is performed
/// _fromTo is a pair of coordinates that represent which piece performs the move and where to click to perform the move respectively.
/// </summary>
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

    /// <summary>
    /// Performs all actions in the internal list _actions.
    /// If any single action fails, the whole move fails.
    /// </summary>
    /// <param name="moveWorker">The board state to perform the actions on.</param>
    /// 
    /// <returns>A GameEvent that determines whether the move succeeded or was invalid.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker)
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

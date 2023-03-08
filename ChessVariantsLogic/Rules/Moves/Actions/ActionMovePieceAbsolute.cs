using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Moves.Actions;
/// <summary>
/// When performed this action will forcefully move a piece on the board according to the given absolute positions.
/// </summary>
public class ActionMovePieceAbsolute : IAction
{
    private readonly string _fromTo;

    public ActionMovePieceAbsolute(string fromTo)
    {
        _fromTo = fromTo;
    }

    /// <summary>
    /// Forcefully move a piece on the board according to the given absolute positions.
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">Since the positions of the move is already given in absolute form this variable is not used.</param>
    /// 
    /// <returns>A GameEvent that represents whether or not the action was successfully performed.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker, string performingPiecePosition)
    {
        return moveWorker.ForceMove(_fromTo);
    }
}

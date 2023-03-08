using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Moves.Actions;
/// <summary>
/// When performed this action will place an UnoccupiedSquareIdentifier at the target position.
/// </summary>
public class ActionDeletePieceAbsolute : IAction
{
    private readonly string _at;

    public ActionDeletePieceAbsolute(string at)
    {
        _at = at;
    }


    /// <summary>
    /// Delete a piece on the board according to the given absolute position.
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="performingPiecePosition">Since the position of the piece to be deleted is already given in absolute form, this variable is not used.</param>
    /// 
    /// <returns>A GameEvent that represents whether or not the action was successfully performed.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker, string performingPiecePosition)
    {
        bool performedSucessfully = moveWorker.Board.Insert(Constants.UnoccupiedSquareIdentifier, _at);
        if (performedSucessfully)
            return GameEvent.MoveSucceeded;
        else
            return GameEvent.InvalidMove;
    }
}

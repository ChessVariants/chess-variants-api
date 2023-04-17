using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Moves.Actions;
/// <summary>
/// When performed this action will place an UnoccupiedSquareIdentifier at the target position.
/// </summary>
public class ActionDeletePiece : ActionSetPiece
{
    public ActionDeletePiece(IPosition at) : base(at, Constants.UnoccupiedSquareIdentifier)
    {
    }

}

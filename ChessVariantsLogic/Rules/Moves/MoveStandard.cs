using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;

using Predicates;

public class MoveStandard : SpecialMove
{
    public MoveStandard(string fromTo) : base(new List<IAction> { new ActionMovePieceAbsolute(fromTo) }, fromTo)
    {

    }


}
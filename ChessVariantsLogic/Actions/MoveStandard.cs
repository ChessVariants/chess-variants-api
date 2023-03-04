namespace ChessVariantsLogic.Actions;

using Predicates;

public class MoveStandard : Move
{
    public MoveStandard(string fromTo) : base(new List<IAction> { new ActionMovePiece(fromTo) }, new Const(true), fromTo)
    {
        
    }


}
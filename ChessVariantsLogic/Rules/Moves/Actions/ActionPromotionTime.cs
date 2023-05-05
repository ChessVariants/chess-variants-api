using System;
namespace ChessVariantsLogic.Rules.Moves.Actions;

public class ActionPromotionTime : Action
{
    public ActionPromotionTime()
    {
    }

    public override GameEvent Perform(MoveWorker moveWorker, string moveCoordinates)
    {
        return GameEvent.Promotion;
    }
}


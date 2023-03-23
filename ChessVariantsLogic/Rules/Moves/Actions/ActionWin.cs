namespace ChessVariantsLogic.Rules.Moves.Actions;
public class ActionWin : ActionGameEvent
{
    public ActionWin(Player player) : base(player == Player.White ? GameEvent.WhiteWon : GameEvent.BlackWon)
    {
    }

}

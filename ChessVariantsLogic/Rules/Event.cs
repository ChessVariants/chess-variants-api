
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Moves.Actions;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules;
public class Event
{
    private readonly IPredicate _predicate;
    private readonly ISet<IAction> _actions;
    private readonly RelativeTo _relativeTo;

    public Event(IPredicate predicate, ISet<IAction> actions, RelativeTo relativeTo)
    {
        _predicate = predicate;
        _actions = actions;
        _relativeTo = relativeTo;
    }

    public bool ShouldRun(BoardTransition lastTransition)
    {
        return _predicate.Evaluate(lastTransition);
    }

    public GameEvent Run(MoveWorker moveWorker)
    {
        Move lastMove = moveWorker.getLastMove();

        var (from, to) = MoveWorker.ParseMove(lastMove.FromTo);

        string move = _relativeTo == RelativeTo.FROM ? from + from : to + to;
        
        Move moveEvent = new Move(_actions, move, lastMove.PieceClassifier);

        return moveEvent.Perform(moveWorker);
    }


    public static Event PromotionEvent(Player player, int boardHeight)
    {
        string pawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        string queenIdentifier = player == Player.White ? Constants.WhiteQueenIdentifier : Constants.BlackQueenIdentifier;
        int rank = player == Player.White ? 0 : (boardHeight-1);

        IPredicate pawnMoved = new PieceMoved(pawnIdentifier);
        IPredicate pawnAtRank = new PositionHasRank(new PositionRelative(0, 0), rank, RelativeTo.TO);

        ISet<IAction> actions = new HashSet<IAction> { new ActionSetPiece(new PositionRelative(0, 0), queenIdentifier) };

        return new Event(pawnMoved & pawnAtRank, actions, RelativeTo.TO);
    }


    public static Event ExplosionEvent(Player player, IPosition pos)
    {
        string oppositePawnIdentifier = player == Player.White ? Constants.BlackPawnIdentifier : Constants.WhitePawnIdentifier;
        IPredicate pieceCaptured = player == Player.White ? new PieceCaptured("ANY_BLACK") : new PieceCaptured("ANY_WHITE");
        IPredicate oppositePawnAt = new PieceAt(oppositePawnIdentifier, pos, BoardState.NEXT, RelativeTo.TO);

        ISet<IAction> actions = new HashSet<IAction> { new ActionSetPiece(pos, Constants.UnoccupiedSquareIdentifier)};

        return new Event(pieceCaptured & !oppositePawnAt, actions, RelativeTo.TO);
    }


}

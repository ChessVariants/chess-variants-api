
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Moves.Actions;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents an event that can happen on the board. The Event will evaluate a predicate over the last boardTransition performed and then perform the internal set of actions.
/// </summary>
public class Event
{
    private readonly IPredicate _predicate;
    private readonly ISet<Action> _actions;

    public Event(IPredicate predicate, ISet<Action> actions)
    {
        _predicate = predicate;
        _actions = actions;
    }

    /// <summary>
    /// Evaluates if the internal predicate holds for the given BoardTransition.
    /// </summary>
    /// <param name="lastTransition">The last transition that was made on the board.</param>
    /// <returns>True if the predicate holds, otherwise false.</returns>
    public bool ShouldRun(BoardTransition lastTransition)
    {
        string fromTo = lastTransition.Move.FromTo;
        
        Move moveEvent = new Move(_actions, fromTo, lastTransition.Move.PieceClassifier);
        
        BoardTransition newTransition = new BoardTransition(lastTransition.NextState, moveEvent);

        return _predicate.Evaluate(lastTransition) && newTransition.IsValid();
    }

    /// <summary>
    /// Runs the event on the given <paramref name="moveWorker"/>. This does not take into account whether the event actually should be run.
    /// </summary>
    /// <param name="moveWorker">The MoveWorker we want to run the event on.</param>
    /// <returns>A GameEvent that represents whether or not the event was successfully run./returns>
    public ISet<GameEvent> Run(MoveWorker moveWorker)
    {
        var events = new HashSet<GameEvent>();
        var lastMove = moveWorker.GetLastMove();
        if (lastMove == null) throw new NullReferenceException("Can't run event if movelog is empty");

        foreach (var action in _actions)
        {
            var gameEvent = action.Perform(moveWorker, lastMove.From, lastMove.To);
            events.Add(gameEvent);
        }
        return events;
    }

    
    /// <summary>
    /// Constructs a promotion event for all pawns of the given player
    /// </summary>
    public static Event PromotionEvent(Player player, int boardHeight)
    {
        string pawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        string queenIdentifier = player == Player.White ? Constants.WhiteQueenIdentifier : Constants.BlackQueenIdentifier;
        int rank = player == Player.White ? (boardHeight) : 1;

        IPredicate pawnMoved = new PieceMoved(pawnIdentifier);
        IPredicate pawnAtRank = new PositionHasRank(new PositionRelative(0, 0), rank, RelativeTo.TO);

        ISet<Action> actions = new HashSet<Action> { new ActionSetPiece(new PositionRelative(0, 0), queenIdentifier, RelativeTo.TO) };

        return new Event(pawnMoved & pawnAtRank, actions);
    }


    /// <summary>
    /// Constructs an event that removes a piece at the given <paramref name="position"/> when the given <paramref name="player"/> captures a piece.
    /// If <paramref name="position"/> is relative, it will be calculated relative to the captured piece's position.
    /// </summary>
    public static Event ExplosionEvent(Player player, IPosition position, bool destroyPawn)
    {
        IPredicate pieceCaptured = player == Player.White ? new PieceCaptured("ANY_BLACK") : new PieceCaptured("ANY_WHITE");

        IPredicate whitePawnAt = new PieceAt(Constants.WhitePawnIdentifier, position, BoardState.NEXT, RelativeTo.TO);
        IPredicate blackPawnAt = new PieceAt(Constants.BlackPawnIdentifier, position, BoardState.NEXT, RelativeTo.TO);

        ISet<Action> actions = new HashSet<Action> { new ActionSetPiece(position, Constants.UnoccupiedSquareIdentifier, RelativeTo.TO) };

        IPredicate predicate = pieceCaptured;
        if(!destroyPawn)
            predicate &= !(whitePawnAt | blackPawnAt);

        return new Event(predicate, actions);
    }

    public static Event WinEvent(Player player, IPredicate winPredicate)
    {
        return new Event(winPredicate, new HashSet<Action>() { new ActionWin(player) });
    }
    public static Event TieEvent(IPredicate tiePredicate)
    {
        return new Event(tiePredicate, new HashSet<Action>() { new ActionTie() });
    }
}

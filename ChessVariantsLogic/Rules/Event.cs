
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
    public readonly List<Action> Actions;

    public Event(IPredicate predicate, List<Action> actions)
    {
        _predicate = predicate;
        Actions = actions;
    }

    /// <summary>
    /// Evaluates if the internal predicate holds for the given BoardTransition.
    /// </summary>
    /// <param name="lastTransition">The last transition that was made on the board.</param>
    /// <returns>True if the predicate holds, otherwise false.</returns>
    public bool ShouldRun(BoardTransition lastTransition)
    {
        return _predicate.Evaluate(lastTransition);
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

        List<Action> actions = new List<Action> { new ActionSetPiece(new PositionRelative(0, 0), queenIdentifier, RelativeTo.TO) };

        return new Event(pawnMoved & pawnAtRank, actions);
    }


    /// <summary>
    /// Constructs an event that removes a piece at the given <paramref name="position"/> when the given <paramref name="player"/> captures a piece.
    /// If <paramref name="position"/> is relative, it will be calculated relative to the captured piece's position.
    /// </summary>
    public static Event ExplosionEvent(Player player, IPosition position, bool destroyPawn)
    {
        IPredicate pieceCaptured = player == Player.White ? new PieceCaptured("BLACK") : new PieceCaptured("WHITE");

        IPredicate whitePawnAt = new PieceAt(Constants.WhitePawnIdentifier, position, BoardState.NEXT, RelativeTo.TO);
        IPredicate blackPawnAt = new PieceAt(Constants.BlackPawnIdentifier, position, BoardState.NEXT, RelativeTo.TO);

        List<Action> actions = new List<Action> { new ActionSetPiece(position, Constants.UnoccupiedSquareIdentifier, RelativeTo.TO) };

        var predicate = pieceCaptured;
        if(!destroyPawn)
            predicate &= !(whitePawnAt | blackPawnAt);

        return new Event(predicate, actions);
    }

    public static Event WinEvent(Player player, IPredicate winPredicate)
    {
        return new Event(winPredicate, new List<Action>() { new ActionWin(player) });
    }
    public static Event TieEvent(IPredicate tiePredicate)
    {
        return new Event(tiePredicate, new List<Action>() { new ActionTie() });
    }
}

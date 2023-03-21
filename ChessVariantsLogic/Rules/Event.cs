﻿
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
    private readonly ISet<IAction> _actions;
    private readonly RelativeTo _relativeTo;

    public Event(IPredicate predicate, ISet<IAction> actions, RelativeTo relativeTo)
    {
        _predicate = predicate;
        _actions = actions;
        _relativeTo = relativeTo;
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
    /// Runs the event on the given <paramref name="moveWorker"/>. This does not take into account whether the event actually should be run.
    /// </summary>
    /// <param name="moveWorker">The MoveWorker we want to run the event on.</param>
    /// <returns>A GameEvent that represents whether or not the event was successfully run./returns>
    public GameEvent Run(MoveWorker moveWorker)
    {
        Move? lastMove = moveWorker.getLastMove();
        if (lastMove == null) return GameEvent.InvalidMove;

        Tuple<string, string>? fromTo = MoveWorker.ParseMove(lastMove.FromTo);
        if(fromTo == null) return GameEvent.InvalidMove;

        // Here we utilize the Move class to avoid code repetition.
        // This is almost a bit of a hack though, so it might have to be changed in the future.

        string move = _relativeTo == RelativeTo.FROM ? fromTo.Item1 + fromTo.Item1 : fromTo.Item2 + fromTo.Item2;

        Move moveEvent = new Move(_actions, move, lastMove.PieceClassifier);

        return moveEvent.Perform(moveWorker);
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

        ISet<IAction> actions = new HashSet<IAction> { new ActionSetPiece(new PositionRelative(0, 0), queenIdentifier) };

        return new Event(pawnMoved & pawnAtRank, actions, RelativeTo.TO);
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

        ISet<IAction> actions = new HashSet<IAction> { new ActionSetPiece(position, Constants.UnoccupiedSquareIdentifier)};

        IPredicate predicate = pieceCaptured;
        if(!destroyPawn)
            predicate &= !(whitePawnAt | blackPawnAt);

        return new Event(predicate, actions, RelativeTo.TO);
    }


}
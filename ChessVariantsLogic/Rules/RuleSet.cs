using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Moves.Actions;
using ChessVariantsLogic;
namespace ChessVariantsLogic.Rules;

using static ChessVariantsLogic.Game;

/// <summary>
/// This class represents an accumulation of rules defined as <see cref="IPredicate"/>s, separated into move and win rules.
/// </summary>
public class RuleSet
{
    private readonly IPredicate _moveRule;
    private readonly ISet<MoveTemplate> _moveTemplates;
    private readonly ISet<Event> _events;
    private readonly ISet<Event> _stalemateEvents;

    public RuleSet(IPredicate moveRule, ISet<MoveTemplate> moveTemplates, ISet<Event> events, ISet<Event> stalemateEvents)
    {
        _moveRule = moveRule;
        _moveTemplates = moveTemplates;
        _events = events;
        _stalemateEvents = stalemateEvents;
    }

    /// <summary>
    /// From the set of all moves unconditionally possible; calculates the subset of those moves which are accepted by the internal moveRule.
    /// </summary>
    /// <param name="board">The current board state</param>
    /// <param name="sideToPlay">Which side is to make a move</param>
    /// <returns>A dictionary that maps for each move accepted, it's coordinates ("e2e4") to itself. It will only accept moves that the internal moveRule evaluates to true and which do not result in an InvalidMove GameEvent.</returns>
    public IDictionary<string, Move> GetLegalMoves(MoveWorker board, Player sideToPlay)
    {
        var possibleMoves = board.GetAllValidMoves(sideToPlay);
        var acceptedMoves = new Dictionary<string, Move>();
        foreach (var moveCoordinates in possibleMoves)
        {
            var (from, _) = MoveWorker.ParseMove(moveCoordinates);

            var pieceIdentifier = board.Board.GetPieceIdentifier(from);

            Move move = new Move(moveCoordinates, board.GetPieceFromIdentifier(pieceIdentifier));
            BoardTransition transition = new BoardTransition(board, move, _events);

            bool ruleSatisfied = _moveRule.Evaluate(transition);

            if (ruleSatisfied && transition.IsValid())
            {
                acceptedMoves.Add(move.FromTo, move);
            }

        }

        foreach (var moveTemplate in _moveTemplates)
        {
            ISet<Move> specialMoves = moveTemplate.GetValidMoves(board, _moveRule, _events);
            foreach (Move move in specialMoves)
            {
                acceptedMoves.Add(move.FromTo, move);
            }
        }

        return acceptedMoves;
    }

    /// <summary>
    /// Returns true if there are legal moves that can be performed. Used for calculating if it's a stalemate.
    /// </summary>
    /// <param name="board">The current board state</param>
    /// <param name="sideToPlay">Which side is to make a move</param>
    /// <returns>True if at least one legal move can be performed.</returns>
    public bool HasLegalMoves(MoveWorker board, Player sideToPlay)
    {
        var possibleMoves = board.GetAllValidMoves(sideToPlay);
        foreach (var moveCoordinates in possibleMoves)
        {
            var (from, _) = MoveWorker.ParseMove(moveCoordinates);

            var pieceIdentifier = board.Board.GetPieceIdentifier(from);

            Move move = new Move(moveCoordinates, board.GetPieceFromIdentifier(pieceIdentifier));
            BoardTransition transition = new BoardTransition(board, move, _events);

            bool ruleSatisfied = _moveRule.Evaluate(transition);

            if (ruleSatisfied && transition.IsValid())
            {
                return true;
            }

        }

        foreach (var moveTemplate in _moveTemplates)
        {
            if (moveTemplate.HasValidMoves(board, _moveRule, _events))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Loops through all events. If the event's internal predicate holds for the <paramref name="lastTransition"/>, run the event.
    /// </summary>
    /// <param name="lastTransition">The last board transition.</param>
    /// <param name="moveWorker">The MoveWorker to run the event on.</param>
    /// <param name="stalemate">Determines if the stalemate events or the regular events should run.</param>
    /// <returns>A set of GameEvents.</returns>
    public ISet<GameEvent> RunEvents(BoardTransition lastTransition, MoveWorker moveWorker, bool stalemate)
    {
        ISet<GameEvent> gameEvents = new HashSet<GameEvent>();
        ISet<Event> events = stalemate ? _stalemateEvents : _events;
        foreach (var e in events)
        {
            if (e.ShouldRun(lastTransition))
            {
                gameEvents.UnionWith(moveWorker.RunEvent(e));
            }
        }

        gameEvents.IntersectWith(new HashSet<GameEvent>() { GameEvent.Tie, GameEvent.WhiteWon, GameEvent.BlackWon, GameEvent.Promotion });
        if (gameEvents.Count > 1)
            throw new Exception("Running events return multiple win/tie events. Events: " + gameEvents);

        return gameEvents;
    }

}

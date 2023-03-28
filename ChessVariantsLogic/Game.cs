namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Export;
using System;
using System.Collections.Generic;

public class Game {

    private readonly MoveWorker _moveWorker;
    private Player _playerTurn;
    private int _playerMovesRemaining;
    private readonly int _movesPerTurn;
    private readonly RuleSet _whiteRules;
    private readonly RuleSet _blackRules;

    private IDictionary<string, Move> _legalMoves;

    public Game(MoveWorker moveWorker, Player playerToStart, int movesPerTurn, RuleSet whiteRules, RuleSet blackRules)
    {
        _moveWorker = moveWorker;
        _playerTurn = playerToStart;
        _movesPerTurn = _playerMovesRemaining = movesPerTurn;
        _whiteRules = whiteRules;
        _blackRules = blackRules;
        _legalMoves = GetRuleSetForPlayer(_playerTurn).GetLegalMoves(_moveWorker, _playerTurn);
    }

    /// <summary>
    /// Checks whether the given <paramref name="playerRequestingMove"/> is the one to move.
    /// </summary>
    /// <param name="moveCoordinates">The move requested to be made</param>
    /// <param name="playerRequestingMove">The player requesting to make a move</param>
    public ISet<GameEvent> MakeMove(string moveCoordinates, Player? playerRequestingMove)
    {
        _legalMoves.TryGetValue(moveCoordinates, out Move? move);

        if (move == null || playerRequestingMove != _playerTurn)
        {
            return new HashSet<GameEvent>() { GameEvent.InvalidMove };
        }
        return MakeMoveImplementation(move);
    }


    /// <summary>
    /// Performs the given <paramref name="move"/> on the internal move worker. Then runs all events that should be run and subsequently updates _legalMoves dictionary. Returns a set of GameEvents.
    /// </summary>
    /// <param name="move">The move requested to be made</param>
    /// <returns>GameEvent of what happened in the game</returns>
    private ISet<GameEvent> MakeMoveImplementation(Move move)
    {
        var currentPlayer = _playerTurn;
        var opponent = _playerTurn == Player.White ? Player.Black : Player.White;

        BoardTransition boardTransition = new BoardTransition(_moveWorker, move);

        // Perform move and add GameEvents to event set

        var events = _moveWorker.PerformMove(move);

        // Run events for current player and add GameEvents to event set

        events.UnionWith(RunEventsForPlayer(currentPlayer, boardTransition));

        // If opponent has no legal moves, run opponent's stalemate events

        if (!HasLegalMoves(opponent))
            events.UnionWith(RunStalemateEventsForPlayer(opponent, boardTransition));

        // Calculate new player

        DecrementPlayerMoves();

        // Update legal moves to new player's legal moves

        UpdateLegalMovesForPlayer(_playerTurn);

        return events;
    }
    public Dictionary<string, List<string>> GetLegalMoveDict()
    {
        var moveDict = new Dictionary<string, List<string>>();
        foreach (var move in _legalMoves)
        {
            var fromTo = MoveWorker.ParseMove(move.Key);
            if (fromTo == null)
            {
                throw new InvalidOperationException($"Could not parse move {move}");
            }

            var moveList = moveDict.GetValueOrDefault(fromTo.Item1, new List<string>());
            if (moveList.Count == 0)
            {
                moveDict[fromTo.Item1] = moveList;
            }
            moveList.Add(fromTo.Item2);

        }
        return moveDict;
    }

    private ISet<GameEvent> RunEventsForPlayer(Player player, BoardTransition lastTransition)
    {
        return GetRuleSetForPlayer(player).RunEvents(lastTransition, _moveWorker, false);
    }

    private ISet<GameEvent> RunStalemateEventsForPlayer(Player player, BoardTransition lastTransition)
    {
        return GetRuleSetForPlayer(player).RunEvents(lastTransition, _moveWorker, true);
    }

    private void UpdateLegalMovesForPlayer(Player player)
    {
        _legalMoves = GetRuleSetForPlayer(player).GetLegalMoves(_moveWorker, player);
    }

    private bool HasLegalMoves(Player player)
    {
        return GetRuleSetForPlayer(player).HasLegalMoves(_moveWorker, player);
    }

    private RuleSet GetRuleSetForPlayer(Player player)
    {
        if (player == Player.White)
            return _whiteRules;
        else if (player == Player.Black)
            return _blackRules;
        else
            throw new ArgumentException("Player can't be None when getting ruleset :" + player);
    }

    /// <summary>
    /// Decrements the number of moves remaining for the current player. If the player has no moves left, switches the player turn and resets the number of moves remaining.
    /// </summary>
    private void DecrementPlayerMoves() {
        _playerMovesRemaining--;
        if (_playerMovesRemaining <= 0) {
            _playerTurn = _playerTurn == Player.White ? Player.Black : Player.White;
            _playerMovesRemaining = _movesPerTurn;
        }
    }

    public string ExportStateAsJson()
    {
        return GameExporter.ExportGameStateAsJson(_moveWorker.Board, _playerTurn, GetLegalMoveDict());
    }
}



public enum GameEvent {
    InvalidMove,
    MoveSucceeded,
    WhiteWon,
    BlackWon,
    Tie
}

public enum Player {
    White,
    Black,
    None
}

public static class PlayerExtensions
{
    public static string AsString(this Player player)
    {
        return player switch
        {
            Player.White => "white",
            Player.Black => "black",
            _ => throw new ArgumentException("Player must be either white or black"),
        };
    }
}
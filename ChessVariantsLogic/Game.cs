namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Export;

using System;
using System.Collections.Generic;
using ChessVariantsLogic.Engine;

public class Game {

    readonly protected MoveWorker _moveWorker;
    readonly protected RuleSet _whiteRules;
    readonly protected RuleSet _blackRules;
    private Player _playerTurn;
    private int _playerMovesRemaining;
    private readonly int _movesPerTurn;
    private AIPlayer? _ai;


    public MoveWorker MoveWorker
    {
        get { return _moveWorker; }
    }
    public RuleSet WhiteRules
    {
        get { return _whiteRules; }
    }
    public RuleSet BlackRules
    {
        get { return _blackRules; }
    }
    public bool ActiveAI => _ai != null;

    private IDictionary<string, Move> _legalMoves;

    public Game(MoveWorker moveWorker, Player playerToStart, int movesPerTurn, RuleSet whiteRules, RuleSet blackRules, AIPlayer? ai=null)
    {
        _moveWorker = moveWorker;
        _playerTurn = playerToStart;
        _movesPerTurn = _playerMovesRemaining = movesPerTurn;
        _whiteRules = whiteRules;
        _blackRules = blackRules;
        _legalMoves = GetRuleSetForPlayer(_playerTurn).GetLegalMoves(_moveWorker, _playerTurn);
        _ai = ai;
    }

    /// <summary>
    /// Checks whether the given <paramref name="playerRequestingMove"/> is the one to move and if the move requested to be made is a legal move.
    /// Then it calls the MakeMoveImplementation method which actually performs the move
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
    /// Performs the given <paramref name="move"/> on the internal move worker. Then runs all events that should be run and subsequently updates _legalMoves dictionary to next player's legal moves. Returns a set of GameEvents that indicate what happened during the move and events.
    /// </summary>
    /// <param name="move">The move requested to be made</param>
    /// <returns>A set of GameEvents of what happened in the game</returns>
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

    public void AssignAI(AIPlayer ai)
    {
        _ai = ai;
    }

    public ISet<GameEvent> MakeAIMove()
    {
        if (_ai == null)
        {
            throw new InvalidOperationException("Unable to make AI move as there is no AI assigned to this game.");
        }
        var bestMove = _ai.SearchMove(this);
        return MakeMove(bestMove.FromTo, _ai.PlayingAs);
    }

    public bool AIShouldMakeMove()
    {
        return _ai != null && _playerTurn == _ai.PlayingAs;
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

    public GameState ExportState()
    {
        return GameExporter.ExportGameState(_moveWorker.Board, _playerTurn, GetLegalMoveDict());
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





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

    private IDictionary<string, Move> validMoves;

    public Game(MoveWorker moveWorker, Player playerToStart, int movesPerTurn, RuleSet whiteRules, RuleSet blackRules)
    {
        _moveWorker = moveWorker;
        _playerTurn = playerToStart;
        _movesPerTurn = _playerMovesRemaining = movesPerTurn;
        _whiteRules = whiteRules;
        _blackRules = blackRules;
        validMoves = GetRuleSetForPlayer(_playerTurn).GetLegalMoves(_moveWorker, _playerTurn);
    }

    /// <summary>
    /// Checks whether the given <paramref name="playerRequestingMove"/> is the one to move.
    /// </summary>
    /// <param name="moveCoordinates">The move requested to be made</param>
    /// <param name="playerRequestingMove">The player requesting to make a move</param>
    public ISet<GameEvent> MakeMove(string moveCoordinates, Player? playerRequestingMove)
    {
        validMoves.TryGetValue(moveCoordinates, out Move? move);

        if (move == null || playerRequestingMove != _playerTurn)
        {
            return new HashSet<GameEvent>() { GameEvent.InvalidMove };
        }
        return MakeMoveImplementation(move);
    }


    private ISet<GameEvent> MakeMoveImplementation(Move move)
    {
        Player currentPlayer = _playerTurn;
        Player opponent = _playerTurn == Player.White ? Player.Black : Player.White;

        RuleSet currentPlayerRuleSet = GetRuleSetForPlayer(currentPlayer);
        RuleSet opponentRuleSet = GetRuleSetForPlayer(opponent);

        BoardTransition boardTransition = new BoardTransition(_moveWorker, move);

        ISet<GameEvent> events = move.Perform(_moveWorker);

        ISet<GameEvent> gameEventsFromEvents = currentPlayerRuleSet.RunEvents(boardTransition, _moveWorker, true);

        events.UnionWith(gameEventsFromEvents);

        validMoves = opponentRuleSet.GetLegalMoves(_moveWorker, opponent);

        bool movesLeft = validMoves.Count() > 0;

        if (!movesLeft)
        {
            ISet<GameEvent> gameEventsFromEventsNoMoves = opponentRuleSet.RunEvents(boardTransition, _moveWorker, movesLeft);
            events.UnionWith(gameEventsFromEventsNoMoves);
        }

        Player lastPlayer = _playerTurn;
        DecrementPlayerMoves();

        bool stillSamePlayer = _playerTurn == lastPlayer;
        if (stillSamePlayer)
        {
            validMoves = currentPlayerRuleSet.GetLegalMoves(_moveWorker, _playerTurn);
        }

        return events;
    }

    public Dictionary<string, List<string>> GetLegalMoveDict()
    {
        var moveDict = new Dictionary<string, List<string>>();
        var moves = validMoves;
        foreach (var move in moves)
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
    /// Checks whether the given <paramref name="moveCoordinates"/> is valid and if so does the move and return correct GameEvent.
    /// </summary>
    /// <param name="moveCoordinates">The move requested to be made</param>
    /// <returns>GameEvent of what happened in the game</returns>
    
    /*
    private GameEvent MakeMoveImpl(string moveCoordinates) {
        IEnumerable<Move> validMoves;

        if (_playerTurn == Player.White) {
            validMoves = _whiteRules.GetLegalMoves(_moveWorker, _playerTurn);
        } else {
            validMoves = _blackRules.GetLegalMoves(_moveWorker, _playerTurn);
        }


        Move? move = GetMove(validMoves, moveCoordinates);
        if (move == null) return GameEvent.InvalidMove;
        if (validMoves.Contains(move)) {


            BoardTransition transition = new BoardTransition(_moveWorker.CopyBoardState(), move);

            GameEvent gameEvent = move.Perform(_moveWorker);

            if (_playerTurn == Player.White)
            {
                _whiteRules.RunEvents(transition, _moveWorker);
            }
            else
            {
                _blackRules.RunEvents(transition, _moveWorker);
            }

            if (gameEvent == GameEvent.InvalidMove)
                return gameEvent;


            /// TODO: Check for a tie


            if (false)
                return GameEvent.Tie;

            DecrementPlayerMoves();
            return gameEvent;
        }
        return GameEvent.InvalidMove;
    }
    */
    private Move? GetMove(IEnumerable<Move> validMoves, string moveCoords)
    {
        foreach(Move move in validMoves)
        {
            if(move.FromTo.Equals(moveCoords))
            {
                return move;
            }
        }
        return null;
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
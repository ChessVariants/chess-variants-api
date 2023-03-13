namespace ChessVariantsLogic;

using ChessVariantsLogic.Export;
using Predicates;

public class Game {

    private readonly IBoardState _boardState;
    private Player _playerTurn;
    private int _playerMovesRemaining;
    private readonly int _movesPerTurn;
    private readonly RuleSet _whiteRules;
    private readonly RuleSet _blackRules;

    public Game(IBoardState boardState, Player playerToStart, int movesPerTurn, RuleSet whiteRules, RuleSet blackRules)
    {
        _boardState = boardState;
        _playerTurn = playerToStart;
        _movesPerTurn = _playerMovesRemaining = movesPerTurn;
        _whiteRules = whiteRules;
        _blackRules = blackRules;
    }

    /// <summary>
    /// Checks whether the given <paramref name="playerRequestingMove"/> is the one to move.
    /// </summary>
    /// <param name="move">The move requested to be made</param>
    /// <param name="playerRequestingMove">The player requesting to make a move</param>
    public GameEvent MakeMove(string move, Player? playerRequestingMove)
    {
        if (playerRequestingMove != _playerTurn)
        {
            return GameEvent.InvalidMove;
        }
        return MakeMoveImpl(move);
    }

    /// <summary>
    /// Checks whether the given <paramref name="move"/> is valid and if so does the move and return correct GameEvent.
    /// </summary>
    /// <param name="move">The move requested to be made</param>
    /// <returns>GameEvent of what happened in the game</returns>
    private GameEvent MakeMoveImpl(string move)
    {
        ISet<string> validMoves;
        if (_playerTurn == Player.White) 
        {
            validMoves = _whiteRules.ApplyMoveRule(_boardState, _playerTurn);
        } else 
        {
            validMoves = _blackRules.ApplyMoveRule(_boardState, _playerTurn);
        }
        if (validMoves.Contains(move)) {
        
            GameEvent gameEvent = _boardState.Move(move);

            if(gameEvent == GameEvent.InvalidMove) {
                return gameEvent;
            }

            if (GameIsWon()) {
                if (_playerTurn == Player.White) {
                    return GameEvent.WhiteWon;
                } else {
                    return GameEvent.BlackWon;
                }
            }

            if(_playerMovesRemaining == 1 & GameIsTie()) {
                return GameEvent.Tie;
            }
            
            DecrementPlayerMoves();
            return gameEvent;
        }
        return GameEvent.InvalidMove;
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

    /// <summary>
    /// Checks whether the game is won.
    /// </summary>
    /// <returns>bool of value true if the game is won</returns>
    private bool GameIsWon(){
        if (_playerTurn == Player.White) {
            return _whiteRules.ApplyWinRule(_boardState, _playerTurn);
        } else {
            return _blackRules.ApplyWinRule(_boardState, _playerTurn);
        }
    }

    /// <summary>
    /// Checks whether the game is tied.
    /// </summary>
    /// <returns>bool of value true if the game is tied</returns>
    private bool GameIsTie() {
        ISet<string> nextMoves;
        bool lost;
        if (_playerTurn == Player.Black) {
            nextMoves = _whiteRules.ApplyMoveRule(_boardState, _playerTurn);
            lost = _blackRules.ApplyWinRule(_boardState, _playerTurn);
        } else {
            nextMoves = _blackRules.ApplyMoveRule(_boardState, _playerTurn);
            lost = _blackRules.ApplyWinRule(_boardState, _playerTurn);
        }
        if (nextMoves.Count == 0 & !lost) {
            return true;
        }
        return false;
    }

    public string ExportStateAsJson()
    {
        RuleSet rules = _playerTurn == Player.White ? _whiteRules : _blackRules;
        return GameExporter.ExportGameStateAsJson(_boardState.Board, _playerTurn, rules.GetLegalMoveDict(_playerTurn, _boardState));
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
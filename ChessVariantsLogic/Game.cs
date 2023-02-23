namespace ChessVariantsLogic;

using Predicates;

public class Game {

    private readonly Chessboard _board;
    private Player _playerTurn;
    private int _playerMovesRemaining;
    private readonly int _movesPerTurn;
    private readonly RuleSet _whiteRules;
    private readonly RuleSet _blackRules;

    public Game(Chessboard board, Player playerToStart, int movesPerTurn, RuleSet whiteRules, RuleSet blackRules)
    {
        _board = board;
        _playerTurn = playerToStart;
        _movesPerTurn = _playerMovesRemaining = movesPerTurn;
        _whiteRules = whiteRules;
        _blackRules = blackRules;
    }

    public GameEvent MakeMove(string move, Player playerRequestingMove) {
        if (playerRequestingMove != _playerTurn) {
            return GameEvent.InvalidMove;
        }
        return MakeMoveImpl(move);
    }

    private GameEvent MakeMoveImpl(string move) {
        IEnumerable<string> validMoves = _board.GetAllValidMoves(_playerTurn);
        // Filter out moves using rules
        if (validMoves.Contains(move)) {
            var moveWasPossible = _board.Move(move);

            if (false) { // check if game is won via rules
                return GameEvent.Tie;
            }
            
            if (moveWasPossible) {
                decrementPlayerMoves();
            }
        }
        return GameEvent.InvalidMove;
    }

    private void decrementPlayerMoves() {
        _playerMovesRemaining--;
        if (_playerMovesRemaining <= 0) {
            _playerTurn = _playerTurn == Player.White ? Player.Black : Player.White;
            _playerMovesRemaining = _movesPerTurn;
        }
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
    Black
}
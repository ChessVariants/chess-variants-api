using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;

public class NegaMax : IMoveFinder
{
    private Move _nextMove;
    private const int WhiteToMove = 1;
    private const int BlackToMove = -1;
    private int _alpha = -100000;
    private int _beta = 100000;
    private int _score;
    private List<Piece> _pieces;
    private bool _blackWon = false;
    private bool _whiteWon = false;
    private bool _draw = false;
    private Stack<IDictionary<string, Move>> _legalMovesLog = new Stack<IDictionary<string, Move>>();

    private PieceValue _pieceValue;
    public NegaMax(PieceValue pieceValue)
    {
        _pieceValue = pieceValue;
    }


    /// <summary>
    /// Calculates the best move for a player using negaMax algorithm
    /// </summary>
    /// <param name="depth"> Number of moves to look ahead </param>
    /// <param name="game"> The game that is beeing played </param>
    /// <param name="player"> The player to move </param>
    /// <returns> The best move for the player </returns>
    public Move FindBestMove(int depth, Game game, Player player)
    {
        int turnMultiplier;
        var tmp_legalMoves = game.LegalMoves;
        var tmp_playerTurn = game.PlayerTurn;

        if (player.Equals(Player.White))
        {
            turnMultiplier = WhiteToMove;
            game.PlayerTurn = Player.White;
        }
        else
        {
            turnMultiplier = BlackToMove;
            game.PlayerTurn = Player.Black;
        }
        NegaMaxAlgorithm(depth, turnMultiplier, depth, _alpha, _beta, game);

        game.LegalMoves = tmp_legalMoves;
        game.PlayerTurn = tmp_playerTurn;
        if (_nextMove == null)
        {
            throw new ArgumentNullException("no valid nextMove found!");
        }
        return _nextMove;
    }



    private int NegaMaxAlgorithm(int currentDepth, int turnMultiplier, int maxDepth, int alpha, int beta, Game game)
    {
        if (currentDepth == 0)
        {
            return turnMultiplier * ScoreBoard(game.MoveWorker);
        }

        int max = -100000;

        UpdatePlayerTurn(game, turnMultiplier);

        var validMoves = game.LegalMoves.Values;
        foreach (var move in validMoves)
        {
            var legalMoves = SaveGameState(game);
            var events = MakeAiMove(game, move.FromTo, game.PlayerTurn, legalMoves, game.PlayerTurn);

            UpdatePlayerVictory(events);

            _score = -NegaMaxAlgorithm(currentDepth - 1, -turnMultiplier, maxDepth, -beta, -alpha, game);

            if (_score > max)
            {
                max = _score;
                if (currentDepth == maxDepth)
                {
                    _nextMove = move;
                }
            }
            game.MoveWorker.undoMove();
            game.LegalMoves = _legalMovesLog.Pop();
            if (_score > alpha)
                alpha = _score;
            if (alpha >= beta)
                break;
        }
        return max;
    }

    public int ScoreBoard(MoveWorker moveWorker)
    {
        int score = 0;
        if (_blackWon)
        {
            _blackWon = false;
            return -1000000;
        }
        if (_whiteWon)
        {
            _whiteWon = false;
            return 1000000;
        }
        if (_draw)
        {
            _draw = false;
            return 0;
        }

        for (int row = 0; row < moveWorker.Board.Rows; row++)
        {
            for (int col = 0; col < moveWorker.Board.Cols; col++)
            {
                var piece = moveWorker.Board.GetPieceIdentifier(row, col);
                if (piece != null)
                {
                    score += _pieceValue.GetValue(piece);
                }
            }
        }
        return score;
    }

    public ISet<GameEvent> MakeAiMove(Game game, string moveCoordinates, Player? playerRequestingMove, IDictionary<string, Move> _legalMoves, Player _playerTurn)
    {
        _legalMoves.TryGetValue(moveCoordinates, out Move? move);

        if (move == null || playerRequestingMove != _playerTurn)
        {
            return new HashSet<GameEvent>() { GameEvent.InvalidMove };
        }
        return MakeAiMoveImplementation(move, game, _playerTurn);
    }

    private ISet<GameEvent> MakeAiMoveImplementation(Move move, Game game, Player _playerTurn)
    {
        var currentPlayer = _playerTurn;
        var opponent = _playerTurn == Player.White ? Player.Black : Player.White;

        BoardTransition boardTransition = new BoardTransition(game.MoveWorker, move);

        // Perform move and add GameEvents to event set

        var events = game.MoveWorker.PerformMove(move);

        // Run events for current player and add GameEvents to event set

        events.UnionWith(game.RunEventsForPlayer(_playerTurn, boardTransition));

        // If opponent has no legal moves, run opponent's stalemate events

        if (!game.HasLegalMoves(opponent))
            events.UnionWith(game.RunStalemateEventsForPlayer(opponent, boardTransition));


        return events;
    }

    private bool HasWhiteWon(ISet<GameEvent> events)
    {
        return events.Contains(GameEvent.WhiteWon);
    }

    private bool HasBlackWon(ISet<GameEvent> events)
    {
        return events.Contains(GameEvent.BlackWon);
    }

    private bool HasDrawn(ISet<GameEvent> events)
    {
        return events.Contains(GameEvent.Tie);
    }

    private void UpdatePlayerVictory(ISet<GameEvent> events)
    {

        if (HasWhiteWon(events))
        {
            _whiteWon = true;
        }
        if (HasBlackWon(events))
        {
            _blackWon = true;
        }
        if (HasDrawn(events))
        {
            _draw = true;
        }
    }

    private void UpdatePlayerTurn(Game game, int turnMultiplier)
    {
        if (turnMultiplier == WhiteToMove)
        {
            game.LegalMoves = game.WhiteRules.GetLegalMoves(game.MoveWorker, Player.White);
            game.PlayerTurn = Player.White;
        }
        else
        {
            game.LegalMoves = game.BlackRules.GetLegalMoves(game.MoveWorker, Player.Black);
            game.PlayerTurn = Player.Black;
        }
    }

    private Dictionary<string, Move> SaveGameState(Game game)
    {
        var boardTmp = game.MoveWorker.Board.CopyBoard();
        game.MoveWorker.StateLog.Push(boardTmp);

        var legalMoves = game.LegalMoves.ToDictionary(entry => entry.Key,
                                           entry => entry.Value);

        _legalMovesLog.Push(legalMoves);
        return legalMoves;
    }
}



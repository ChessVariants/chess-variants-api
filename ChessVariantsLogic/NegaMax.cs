namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Export;
using ChessVariantsLogic;


using System;
using System.Collections.Generic;

public class NegaMax
{
    private Move _nextMove;
    private int _whiteToMove = 1;
    private int _blackToMove = -1;
    private int _alpha = -100000;
    private int _beta = 100000;
    private int _score;
    private List<Piece> _pieces;
    private bool _blackWon = false;
    private bool _whiteWon = false;
    private bool _draw = false;
    public Stack<IDictionary<string,Move>> _legalMovesLog = new Stack<IDictionary<string, Move>>();

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
        var tmp_legalMoves = game._legalMoves;
        var tmp_playerTurn = game._playerTurn;
        
        if (player.Equals(Player.White))
        {
            turnMultiplier = _whiteToMove;
            game._playerTurn = Player.White;
        }
        else
        {
            turnMultiplier = _blackToMove;
            game._playerTurn = Player.Black;
        }
        negaMax(depth, turnMultiplier, depth, _alpha, _beta, game);

        game._legalMoves = tmp_legalMoves;
        game._playerTurn = tmp_playerTurn;
        if (_nextMove == null)
        {
            throw new ArgumentNullException("no valid nextMove found!");
        }
        return _nextMove;
    }



    private int negaMax(int currentDepth, int turnMultiplier, int maxDepth, int alpha, int beta, Game game)
    {
        if (currentDepth == 0)
        {
            return turnMultiplier * ScoreBoard(game.MoveWorker);
        }

        int max = -100000;

        updatePlayerTurn(game, turnMultiplier);

        var validMoves = game._legalMoves.Values;
        foreach (var move in validMoves)
        {
            var legalMoves = saveGameState(game);
            var events = MakeAiMove(game, move.FromTo, game._playerTurn, legalMoves, game._playerTurn);

            updatePlayerVictory(events);

            _score = -negaMax(currentDepth - 1, -turnMultiplier, maxDepth, -beta, -alpha, game);

            if (_score > max)
            {
                max = _score;
                if (currentDepth == maxDepth)
                {
                    _nextMove = move;
                }
            }
            game.MoveWorker.undoMove();
            game._legalMoves = _legalMovesLog.Pop();
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
                    score += _pieceValue.getValue(piece);
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
        return makeAiMoveImplementation(move, game, _playerTurn);
    }

    private ISet<GameEvent> makeAiMoveImplementation(Move move, Game game, Player _playerTurn)
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

    private bool hasWhiteWon(ISet<GameEvent> events)
    {
        return events.Contains(GameEvent.WhiteWon);
    }

    private bool hasBlackWon(ISet<GameEvent> events)
    {
        return events.Contains(GameEvent.BlackWon);
    }

    private bool hasDrawn(ISet<GameEvent> events)
    {
        return events.Contains(GameEvent.Tie);
    }

    private void updatePlayerVictory(ISet<GameEvent> events)
    {
        
        if (hasWhiteWon(events))
            {
                _whiteWon = true;
            }
            if (hasBlackWon(events))
            {
                _blackWon = true;
            }
            if (hasDrawn(events))
            {
                _draw = true;
            }
    }

    private void updatePlayerTurn(Game game, int turnMultiplier)
    {
        if (turnMultiplier == _whiteToMove)
        {
            game._legalMoves = game.WhiteRules.GetLegalMoves(game.MoveWorker, Player.White);
            game._playerTurn = Player.White;
        }
        else
        {
            game._legalMoves = game.BlackRules.GetLegalMoves(game.MoveWorker, Player.Black);
            game._playerTurn = Player.Black;
        }
    }

    private Dictionary<string, Move> saveGameState(Game game)
    {
        var boardTmp = game.MoveWorker.Board.CopyBoard();
        game.MoveWorker.StateLog.Push(boardTmp);

        var legalMoves = game._legalMoves.ToDictionary(entry => entry.Key,
                                           entry => entry.Value);

        _legalMovesLog.Push(legalMoves);
        return legalMoves;
    }

    
}
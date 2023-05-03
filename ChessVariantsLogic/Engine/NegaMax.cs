using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;

public class NegaMax : IMoveFinder
{
    private Random _rand;
    private double test = 0;
    private Move _nextMove;
    private int _whiteToMove = 1;
    private int _blackToMove = -1;
    private double _alpha = -100000;
    private double _beta = 100000;
    private double _score;
    private List<Piece> _pieces;
    private bool _blackWon = false;
    private bool _whiteWon = false;
    private bool _draw = false;
    private Stack<IDictionary<string, Move>> _legalMovesLog = new Stack<IDictionary<string, Move>>();
    Random random = new Random();
    Dictionary<(string,int,int),ulong> _zobristKeys = new Dictionary<(string,int,int),ulong>();
    

    private PieceValue _pieceValue;
    public NegaMax(HashSet<Piece> pieces, Chessboard chessboard)
    {
        _pieceValue = new PieceValue(pieces, chessboard);

        initZobristKey(pieces, chessboard);

    }
    private HeatMap _heatMap;



    /// <summary>
    /// Calculates the best move for a player using negaMax algorithm
    /// </summary>
    /// <param name="depth"> Number of moves to look ahead </param>
    /// <param name="game"> The game that is beeing played </param>
    /// <param name="player"> The player to move </param>
    /// <returns> The best move for the player </returns>
    public Move FindBestMove(int depth, Game game, Player player, ScoreVariant scoreVariant)
    {
        int turnMultiplier;
        _heatMap = new HeatMap(game.MoveWorker.Board.Rows, game.MoveWorker.Board.Rows);
        var tmp_legalMoves = game.LegalMoves;
        var tmp_playerTurn = game.PlayerTurn;

        if (player.Equals(Player.White))
        {
            turnMultiplier = _whiteToMove;
            game.PlayerTurn = Player.White;
        }
        else
        {
            turnMultiplier = _blackToMove;
            game.PlayerTurn = Player.Black;
        }
        NegaMaxAlgorithm(depth, turnMultiplier, depth, _alpha, _beta, game, scoreVariant);

        game.LegalMoves = tmp_legalMoves;
        game.PlayerTurn = tmp_playerTurn;
        if (_nextMove == null)
        {
            throw new ArgumentNullException("no valid nextMove found!");
        }
        return _nextMove;
    }



    private double NegaMaxAlgorithm(int currentDepth, int turnMultiplier, int maxDepth, double alpha, double beta, Game game, ScoreVariant scoreVariant)
    {
        if (currentDepth == 0)
        {
            return turnMultiplier * ScoreBoard(game.MoveWorker, game.PlayerTurn, scoreVariant);
        }

        double max = -100000;

        UpdatePlayerTurn(game, turnMultiplier);

        var validMoves = game.LegalMoves.Values;

        List<Move> L = validMoves.ToList();

        Shuffle(L);

        foreach (var move in L)
        {
            var legalMoves = SaveGameState(game);
            var events = MakeAiMove(game, move.FromTo, game.PlayerTurn, legalMoves, game.PlayerTurn);

            UpdatePlayerVictory(events);

            _score = -NegaMaxAlgorithm(currentDepth - 1, -turnMultiplier, maxDepth, -beta, -alpha, game, scoreVariant);

            if (_score > max)
            {
                max = _score;
                if (currentDepth == maxDepth)
                {
                    _nextMove = move;
                    //test = _score;
                }
            }
            game.MoveWorker.UndoMove();
            game.LegalMoves = _legalMovesLog.Pop();
            if (_score > alpha)
                alpha = _score;
            if (alpha >= beta)
                break;
        }
        return max;
    }

    public double ScoreBoard(MoveWorker moveWorker, Player player, ScoreVariant scoreVariant)
    {
        double score = 0;
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
                if (piece != null && piece != "--")
                {
                    score += _pieceValue.getValue(piece);

                    if (moveWorker.GetPieceClassifier(piece).Equals(PieceClassifier.WHITE))
                        score += _heatMap.GetValue(row, col);
                    if (moveWorker.GetPieceClassifier(piece).Equals(PieceClassifier.BLACK))
                        score -= _heatMap.GetValue(row, col);
                }
            }
        }

        double numberOfThreats = moveWorker.GetAllThreatMoves(Player.White).Count() - moveWorker.GetAllThreatMoves(Player.Black).Count();

        double numberOfMoves = moveWorker.GetAllValidMoves(Player.White).Count() - moveWorker.GetAllValidMoves(Player.Black).Count();
        
        score += numberOfThreats/5 + numberOfMoves/30;

        if (scoreVariant.Equals(ScoreVariant.AntiChess))
            score = -score;

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
        if (turnMultiplier == _whiteToMove)
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

    public List<Move> Shuffle(List<Move> listToShuffle)
    {
        _rand = new Random();
        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            var k = _rand.Next(i + 1);
            var value = listToShuffle[k];
            listToShuffle[k] = listToShuffle[i];
            listToShuffle[i] = value;
        }

        return listToShuffle;
    }

    private void initZobristKey(HashSet<Piece> pieces, Chessboard chessboard)
    {
        foreach(var piece in pieces)
        {
            for(int c = 0; c <= chessboard.Cols - 1; c++)
            {
                for(int r = 0; r <= chessboard.Rows - 1; r++)
                {
                    ulong randomKey = (ulong)random.NextLong();
                    _zobristKeys[(piece.PieceIdentifier,c,r)] = randomKey;
                }
            }
        }
    }

    

}
public enum ScoreVariant
    {
        AntiChess,
        RegularChess
    }



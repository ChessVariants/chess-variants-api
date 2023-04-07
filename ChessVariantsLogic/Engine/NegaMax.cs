using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;

public class NegaMax : IMoveFinder
{
    private Move nextMove;
    private int whiteToMove = 1;
    private int blackToMove = -1;
    private int alpha = -10000;
    private int beta = 100000;
    private List<Piece> pieces;

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
    public Move findBestMove(int depth, Game game, Player player)
    {
        int turnMultiplier;
        if (player.Equals(Player.White))
        {
            turnMultiplier = whiteToMove;
        }
        else
        {
            turnMultiplier = blackToMove;
        }
        negaMax(depth, turnMultiplier, depth, alpha, beta, game);

        if (nextMove == null)
        {
            throw new ArgumentNullException("no valid nextMove found!");
        }
        return nextMove;
    }



    private int negaMax(int currentDepth, int turnMultiplier, int maxDepth, int alpha, int beta, Game game)
    {
        if (currentDepth == 0)
        {
            return turnMultiplier * ScoreBoard(game.MoveWorker);
        }
        int max = -100000;
        int score;
        IEnumerable<Move> validMoves;

        if (turnMultiplier == whiteToMove)
        {
            validMoves = game.WhiteRules.GetLegalMoves(game.MoveWorker, Player.White).Values;
        }
        else
        {
            validMoves = game.BlackRules.GetLegalMoves(game.MoveWorker, Player.Black).Values;
        }

        foreach (var move in validMoves)
        {
            var boardTmp = game.MoveWorker.Board.CopyBoard();
            game.MoveWorker.StateLog.Push(boardTmp);
            game.MoveWorker.PerformMove(move);

            score = -negaMax(currentDepth - 1, -turnMultiplier, maxDepth, -beta, -alpha, game);
            if (score > max)
            {
                max = score;
                if (currentDepth == maxDepth)
                {
                    nextMove = move;
                }
            }
            game.MoveWorker.undoMove();
            if (score > alpha)
                alpha = score;
            if (alpha >= beta)
                break;


        }
        return max;
    }

    public int ScoreBoard(MoveWorker moveWorker)
    {
        int score = 0;
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
}

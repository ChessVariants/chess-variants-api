namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Export;
using ChessVariantsLogic;


using System;
using System.Collections.Generic;

public class NegaMax
{
    private Move nextMove;
    private int whiteToMove = 1;
    private int blackToMove = -1;
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
        IEnumerable<Move> validMoves;
        int turnMultiplier;
        if(player.Equals(Player.White))
        {
            validMoves = game.WhiteRules.GetLegalMoves(game.MoveWorker, Player.White).Values;
            turnMultiplier = whiteToMove;
        }
        else
        {
            validMoves = game.BlackRules.GetLegalMoves(game.MoveWorker, Player.Black).Values;
            turnMultiplier = blackToMove;
        }
        negaMax(depth, turnMultiplier, depth, validMoves, game);

        if(nextMove == null)
        {
            throw new ArgumentNullException("no valid nextMove found!");
        }
        return nextMove;
    }

    

    private int negaMax(int currentDepth, int turnMultiplier, int maxDepth, IEnumerable<Move> validMoves, Game game)
    {
        if (currentDepth == 0)
        {
            return turnMultiplier * scoreBoard(game.MoveWorker);
        }
        int max = -1000;
        int score;
        IEnumerable<Move> nextValidMoves;

        foreach (var move in validMoves)
        {
            var boardTmp = game.MoveWorker.Board.CopyBoard();
            game.MoveWorker.StateLog.Push(boardTmp);
            game.MoveWorker.PerformMove(move);
            if (turnMultiplier == blackToMove)
            {
                nextValidMoves = game.WhiteRules.GetLegalMoves(game.MoveWorker, Player.White).Values;
            }
            else
            {
                nextValidMoves = game.BlackRules.GetLegalMoves(game.MoveWorker, Player.Black).Values;
            }
            score = -negaMax(currentDepth - 1, -turnMultiplier, maxDepth, nextValidMoves, game);
            if (score > max)
            {
                max = score;
                if (currentDepth == maxDepth)
                {
                    nextMove = move;
                }
            }
            game.MoveWorker.undoMove();

        }
        return max;
    }

    public int scoreBoard(MoveWorker moveWorker)
    {
        int score = 0;
        for(int row = 0; row < moveWorker.Board.Rows; row++)
        {
            for(int col = 0; col < moveWorker.Board.Cols; col++)
            {
                var piece = moveWorker.Board.GetPieceIdentifier(row,col);
                if(piece != null)
                {
                    score += _pieceValue.getValue(piece);
                }
            }
        }
        return score;
    }
}
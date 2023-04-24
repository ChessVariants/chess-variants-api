namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Export;
using static ChessVariantsLogic.Game;


using System;
public class Perft : Game
{
    private int nodes = 0;

    public Perft(MoveWorker moveWorker, Player playerToStart, int movesPerTurn, RuleSet whiteRules, RuleSet blackRules)
        : base(moveWorker, playerToStart, movesPerTurn, whiteRules, blackRules) {}

    public int Nodes => nodes;

    public void PerftTest(int depth, Player turn)
    {
        
        if (depth == 0)
        {
            nodes++;
            return;
        }
        IEnumerable<Move> validMoves;
        
        if (turn == Player.White)
        {
            validMoves = _whiteRules.GetLegalMoves(_moveWorker, Player.White).Values;
            turn = Player.Black;
        }
        else
        {
            validMoves = _blackRules.GetLegalMoves(_moveWorker, Player.Black).Values;
            turn = Player.White;
        }

        foreach (var move in validMoves)
        {
            var boardTmp = _moveWorker.Board.CopyBoard();
            _moveWorker.StateLog.Push(boardTmp);
            _moveWorker.PerformMove(move);
            PerftTest(depth - 1, turn);
            _moveWorker.UndoMove();
        }
        return;
    }
}
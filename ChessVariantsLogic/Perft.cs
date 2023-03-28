namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Export;
using static ChessVariantsLogic.Game;


using System;
public class Perft
{
    
    private int nodes = 0;
    public int Nodes
    {
        get{ return this.nodes; }
    }
    public void perftTest(Game game, int depth, Player turn)
    {
        
        if (depth == 0)
        {
            nodes++;
            return;
        }
        IEnumerable<Move> validMoves;
        
        if (turn == Player.White)
        {
            validMoves = game._WhiteRules.ApplyMoveRule(game._MoverWorker, Player.White);
            turn = Player.Black;
        }
        else
        {
            validMoves = game._BlackRules.ApplyMoveRule(game._MoverWorker, Player.Black);
            turn = Player.White;
        }

        foreach (var move in validMoves)
        {
            move.Perform(game._MoverWorker);
            perftTest(game,depth - 1, turn);
            game._MoverWorker.undoMove();
        }
        return;
    }
}
using ChessVariantsLogic;
using ChessVariantsLogic.Predicates;
using System;
using System.Numerics;

namespace ChessVariantsLogic.Predicates;


public class ForEvery : IPredicate
{
    private readonly IPredicate _rule;
    private readonly Player _player;
    
    public ForEvery(IPredicate rule, Player player)
    {
        _rule = rule;
        _player = player;
    }
    public bool Evaluate(Chessboard thisBoard, Chessboard _)
    {
        var possibleMoves = thisBoard.GetAllMoves(_player);
        foreach (var move in possibleMoves)
        {
            var nextBoard = thisBoard.CopyBoard();
            nextBoard.Move(move);
            bool ruleSatisfied = _rule.Evaluate(thisBoard, nextBoard);
            if (!ruleSatisfied)
            {
                return false;
            }
        }
        return true;

    }
}
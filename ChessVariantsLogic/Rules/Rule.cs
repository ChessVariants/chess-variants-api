using System;
namespace ChessVariantsLogic.Rules;

public interface Rule
{
    ISet<string> applyRule(Chessboard currentBoard, Chessboard boardCopy, string player);
}


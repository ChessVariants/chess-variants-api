using System;
using ChessVariantsLogic;

public interface IPredicate
{

    bool evaluate(Chessboard thisBoardState, Chessboard nextBoardState);

}

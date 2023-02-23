using System;
using ChessVariantsLogic;

namespace ChessVariantsLogic.Predicates;

public interface IPredicate
{
    bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState);
}

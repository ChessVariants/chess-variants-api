using System;

public interface IPredicate
{

    bool evaluate(string[,] thisBoardState, string[,] nextBoardState);

    bool evaluatesThisBoardState { get; }

}

using System;

public class RoyalChecked : IPredicate
{

    IPredicate antiChessRule = new Operator(PredType.IMPLIES, new IsAttackingPiece(true, 2), new TookPiece());



    private bool evaluatesNextBoardState;

    public RoyalChecked(bool evaluatesNextBoardState, int pieceType)
	{
        this.evaluatesNextBoardState = evaluatesNextBoardState;


    }

    void test()
    {

        IPredicate chessWinRule = new RoyalChecked(true, 0);

        IPredicate chessMoveRule = new Function(FunctionType.NOT, chessWinRule);

    }

    

    public bool evaluate(string[,] thisBoardState, string[,] nextBoardState)
    {
        string[,] board = evaluatesNextBoardState ? nextBoardState : thisBoardState;
        return board[0, 0] == "";
    }
}

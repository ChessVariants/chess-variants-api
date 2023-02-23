using ChessVariantsLogic;
using System;

public class Function : IPredicate
{
    private readonly IPredicate pred;
    private readonly FunctionType functionType;
    
    public Function(FunctionType functionType, IPredicate pred)
	{
        this.pred = pred;
        this.functionType = functionType;
	}

    public bool evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        switch(functionType)
        {
            case FunctionType.ID: return pred.evaluate(thisBoardState, nextBoardState);
            case FunctionType.NOT: return !(pred.evaluate(thisBoardState, nextBoardState));
            case FunctionType.TRUE: return true;
            case FunctionType.FALSE: return false;
            default: return false;
        }

    }
}

public enum FunctionType
{
    ID, NOT, TRUE, FALSE
}
using System;
using ChessVariantsLogic;

namespace ChessVariantsLogic.Predicates;

public interface IPredicate
{
    bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState);
    
    public static IPredicate operator |(IPredicate p, IPredicate q)
        => new Operator(p, OperatorType.OR, q);

    public static IPredicate operator &(IPredicate p, IPredicate q)
        => new Operator(p, OperatorType.AND, q);

    public static IPredicate operator !(IPredicate a)
        => new Operator(OperatorType.NOT, a);

    public static IPredicate operator ^(IPredicate p, IPredicate q)
        => new Operator(p, OperatorType.XOR, q);

    public static IPredicate operator -(IPredicate p, IPredicate q)
        => new Operator(p, OperatorType.IMPLIES, q);
    
}

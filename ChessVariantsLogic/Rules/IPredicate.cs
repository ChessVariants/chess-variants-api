namespace ChessVariantsLogic.Predicates;

/// <summary>
/// Represents a logical predicate which can evaluate two <see cref="Chessboard"/> objects to either true or false.
/// </summary>
public interface IPredicate
{
    /// <summary>
    /// Evaluates two <see cref="Chessboard"/> objects to either true or false according to implementation.
    /// </summary>
    /// <param name="thisBoardState">A chessboard that represents the current state of the game</param>
    /// <param name="nextBoardState">A chessboard that represents a potential future state of the game</param>
    /// <returns>A boolean value according to the implementation</returns>
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

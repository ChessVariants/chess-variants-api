namespace ChessVariantsLogic.Predicates;

/// <summary>
/// Represents a logical operator which can be performed on <see cref="IPredicate"/> objects.
/// </summary>
public class Operator : IPredicate
{
    private readonly OperatorType _type;
    private readonly IPredicate _p;
    private readonly IPredicate _q;

    public Operator(IPredicate p, OperatorType type, IPredicate q)
    {
        if (type == OperatorType.NOT)
        {
            throw new ArgumentException("Operator with only two predicates can not have OperatorType NOT");
        }
        _type = type;
        _p = p;
        _q = q;
    }

    public Operator(OperatorType type, IPredicate p)
    {
        if (type != OperatorType.NOT)
        {
            throw new ArgumentException("Operator with only one predicate must have OperatorType NOT");
        }
        _type = type;
        _p = p;
        _q = new Const(false);
    }

    /// <summary>
    /// Logically evaluates the two internal <see cref="IPredicate"/> objects by the supplied <see cref="Chessboard"/>s and <see cref="OperatorType"/>.
    /// </summary>
    /// <inheritdoc/>
    public bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        switch (_type)
        {
            case OperatorType.AND: return _p.Evaluate(thisBoardState, nextBoardState) && _q.Evaluate(thisBoardState, nextBoardState);
            case OperatorType.OR: return _p.Evaluate(thisBoardState, nextBoardState) || _q.Evaluate(thisBoardState, nextBoardState);
            case OperatorType.IMPLIES: return !(_p.Evaluate(thisBoardState, nextBoardState)) || _q.Evaluate(thisBoardState, nextBoardState);
            case OperatorType.XOR: return _p.Evaluate(thisBoardState, nextBoardState) ^ _q.Evaluate(thisBoardState, nextBoardState);
            case OperatorType.EQUALS: return _p.Evaluate(thisBoardState, nextBoardState) == _q.Evaluate(thisBoardState, nextBoardState);
            case OperatorType.NOT: return !(_p.Evaluate(thisBoardState, nextBoardState));
            default: return false;
        }
    }
}

/// <summary>
/// Enums for the supported logical operators.
/// </summary>
public enum OperatorType
{
    AND, OR, IMPLIES, XOR, EQUALS, NOT
}
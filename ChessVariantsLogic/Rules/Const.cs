namespace ChessVariantsLogic.Predicates;

/// <summary>
/// A predicate which always returns a constant value either true or false.
/// </summary>
public class Const : IPredicate
{
    private readonly bool _value;

    public Const(bool value)
    {
        _value = value;
    }

    /// <summary>
    /// Returns the boolean value specified at creation.
    /// </summary>
    /// <param name="thisBoardState">Irrelevant, only used to satisfy interface</param>
    /// <param name="nextBoardState">Irrelevant, only used to satisfy interface</param>
    /// <returns>the boolean value specified at creation.</returns>
    public bool Evaluate(IBoardState thisBoardState, IBoardState nextBoardState)
    {
        return _value;
    }
}
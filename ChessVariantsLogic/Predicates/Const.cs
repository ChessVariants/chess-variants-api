using ChessVariantsLogic;

namespace ChessVariantsLogic.Predicates;

public class Const : IPredicate
{
    private readonly bool _value;

    public Const(bool value)
    {
        this._value = value;
    }

    public bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        return _value;
    }
}
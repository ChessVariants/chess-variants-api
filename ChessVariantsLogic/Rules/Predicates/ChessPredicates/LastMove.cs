﻿namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate evaluates if the last move is equal to the compare values.
/// THIS IS WIP
/// </summary>
public class LastMove : IPredicate
{
    private readonly Stack<string> _moveLog;
    private readonly IPosition _compareFrom;
    private readonly IPosition _compareTo;

    public LastMove(IPosition compareFrom, IPosition compareTo)
    {
        _moveLog = new Stack<string>();
        _compareFrom = compareFrom;
        _compareTo = compareTo;
    }

    public bool Evaluate(BoardTransition transition)
    {
        if (_moveLog.Count == 0)
            return false;

        string lastMove = _moveLog.Peek();

        string? from = _compareFrom.GetPosition(transition.ThisState, transition.MoveFrom);
        string? to   = _compareTo.GetPosition(transition.ThisState, transition.MoveFrom);
        if (from == null || to == null) return false;

        string compareMove = from + to;

        return compareMove == lastMove;
    }
}
﻿using ChessVariantsLogic.Rules.Moves;
using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
public abstract class MovePredicate : IPredicate
{
    [JsonProperty]
    protected readonly MoveState _moveState;

    public MovePredicate(MoveState moveState)
    {
        _moveState = moveState;
    }

    public abstract bool Evaluate(BoardTransition transition);

    protected Move? GetMove(BoardTransition transition)
    {
        return _moveState == MoveState.THIS ? transition.NextState.GetLastMove() : transition.ThisState.GetLastMove();
    }


}
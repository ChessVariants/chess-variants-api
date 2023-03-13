using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;

using Predicates;

/// <summary>
/// Creates a standard move
/// </summary>

public class MoveStandard : Move
{
    public MoveStandard(string fromTo) : base(null, fromTo)
    {

    }

}
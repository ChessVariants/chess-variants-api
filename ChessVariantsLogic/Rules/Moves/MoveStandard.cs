using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;

using Predicates;

/// <summary>
/// Creates a standard move
/// </summary>

public class MoveStandard : Move
{
    public MoveStandard(string from, string to) : base(new List<IAction> { new ActionMovePiece(new PositionAbsolute(from), new PositionAbsolute(to)) }, from + to)
    {

    }

}
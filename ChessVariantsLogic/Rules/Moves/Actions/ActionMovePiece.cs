using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

namespace ChessVariantsLogic.Rules.Moves.Actions;
/// <summary>
/// When performed this action will forcefully move a piece on the board according to the given positions.
/// </summary>
public class ActionMovePiece : Action
{
    private readonly IPosition _from;
    private readonly IPosition _to;

    public ActionMovePiece(IPosition from, IPosition to, RelativeTo relativeTo = RelativeTo.FROM) : base(relativeTo)
    {
        _from = from;
        _to = to;
    }

    //If from position is not specified, the piece performing the action will be moved.

    public ActionMovePiece(IPosition to) : base(RelativeTo.FROM)
    {
        _from = new PositionRelative(Tuple.Create(0, 0));
        _to = to;
    }

    public ActionMovePiece(string fromTo) : base(RelativeTo.FROM)
    {
        Tuple<string, string>? fromToTuple = MoveWorker.ParseMove(fromTo);
        if (fromToTuple == null) throw new ArgumentException("fromTo is not a proper string: " + fromTo);
        var (from, to) = fromToTuple;
        _from = new PositionAbsolute(from);
        _to = new PositionAbsolute(to);
    }

    /// <summary>
    /// Forcefully move a piece on the board according to the given positions.
    /// If Positions are defined as relative, it will be performed relative to the given <paramref name="pivotPosition"/>
    /// </summary>
    /// <param name="moveWorker">MoveWorker that should perform the action.</param>
    /// <param name="pivotPosition">This variable is used to calculate the from and to positions if they are relative.</param>
    /// 
    /// <returns>A GameEvent that occured when the action was performed.</returns>
    /// 
    protected override GameEvent Perform(MoveWorker moveWorker, string pivotPosition)
    {
        string? from = _from.GetPosition(moveWorker, pivotPosition);
        if (from == null) return GameEvent.InvalidMove;
        string? to = _to.GetPosition(moveWorker, pivotPosition);
        if (to == null) return GameEvent.InvalidMove;
        string move = from + to;
        string? pieceIdentifier = moveWorker.Board.GetPieceIdentifier(to);
        if(pieceIdentifier != null && !pieceIdentifier.Equals(Constants.UnoccupiedSquareIdentifier))
            _capturedPiece = moveWorker.GetPieceFromIdentifier(pieceIdentifier);

        
        return moveWorker.Move(move, true);
    }
}

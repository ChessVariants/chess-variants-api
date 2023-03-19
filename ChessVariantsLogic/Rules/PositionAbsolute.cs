namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents an absolute position on the board.
/// </summary>
public class PositionAbsolute : IPosition
{
    private readonly string _position;

    public PositionAbsolute(string position)
    {
        _position = position;
    }

    /// <summary>
    /// Calculates the position and returns it as a string coordinate.
    /// </summary>
    /// <param name="moveWorker">Since absolute position is already given, this is not needed.</param>
    /// <param name="pivotPosition">Since absolute position is already given, this is not needed.</param>
    /// 
    /// <returns>The calculated position as a string.</returns>
    /// 
    public string? GetPosition(MoveWorker moveWorker, string pivotPosition)
    {
        return _position;
    }
    public Tuple<int, int>? GetPositionTuple(MoveWorker moveWorker, string pivotPosition)
    {
        return moveWorker.Board.ParseCoordinate(_position);
    }
}

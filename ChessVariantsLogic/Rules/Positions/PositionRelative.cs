
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents a relative position on the board that can be calculated at runtime.
/// </summary>
public class PositionRelative : IPosition
{
    [JsonProperty]
    private readonly Tuple<int, int> _relativePosition;
    [JsonProperty]
    private readonly RelativeTo _relativeTo;

    public PositionRelative(int row, int col, RelativeTo relativeTo)
    {
        _relativePosition = Tuple.Create(row, col);
        _relativeTo = relativeTo;
    }
    public PositionRelative(Tuple<int, int> relativePosition, RelativeTo relativeTo)
    {
        _relativePosition = relativePosition;
        _relativeTo = relativeTo;
    }

    /// <summary>
    /// Calculates the position and returns it as a string coordinate.
    /// </summary>
    /// <param name="moveWorker">The MoveWorker containing the board with the position to calculate.</param>
    /// <param name="pivotPosition">The final position will be calculated relative to the pivotPosition</param>
    /// 
    /// <returns>The calculated position as a string.</returns>
    /// 
    public string? GetPosition(MoveWorker moveWorker, string moveCoordinates)
    {
        Tuple<int, int>? finalPosition = GetPositionTuple(moveWorker, moveCoordinates);
        if (finalPosition == null) return null;
        moveWorker.Board.IndexToCoor.TryGetValue(finalPosition, out string? finalPositionString);
        return finalPositionString;
    }

    /// <summary>
    /// Calculates the position and returns it as a tuple coordinate.
    /// </summary>
    /// <param name="moveWorker">The MoveWorker containing the board with the position to calculate.</param>
    /// <param name="pivotPosition">The final position will be calculated relative to the pivotPosition</param>
    /// 
    /// <returns>The calculated position as a tuple.</returns>
    /// 

    public Tuple<int, int>? GetPositionTuple(MoveWorker moveWorker, string moveCoordinates)
    {
        var pivotPosition = ChoosePosition(moveCoordinates);
        if (pivotPosition == null) return null;
        Tuple<int, int>? pivotPositionTuple = moveWorker.Board.ParseCoordinate(pivotPosition);
        if (pivotPositionTuple == null) return null;
        return Tuple.Create(pivotPositionTuple.Item1 + _relativePosition.Item1, pivotPositionTuple.Item2 + _relativePosition.Item2);
    }
    private string? ChoosePosition(string moveCoordinates)
    {
        Tuple<string, string>? moveCoordinatesTuple = MoveWorker.ParseMove(moveCoordinates);
        if (moveCoordinatesTuple == null)
            return null;
        var (from, to) = moveCoordinatesTuple;
        return _relativeTo == RelativeTo.FROM ? from : to;
    }

}


/// <summary>
/// An enum that determines whether predicates that evaluate positions should calculate their positions relative to the From or To variables of the supplied BoardTransition.
/// </summary>
public enum RelativeTo
{
    FROM, TO
}
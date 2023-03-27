﻿
namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents a relative position on the board that can be calculated at runtime.
/// </summary>
public class PositionRelative : IPosition
{
    private readonly Tuple<int, int> _relativePosition;

    public PositionRelative(int row, int col)
    {
        _relativePosition = Tuple.Create(row, col);
    }
    public PositionRelative(Tuple<int, int> relativePosition)
    {
        _relativePosition = relativePosition;
    }

    /// <summary>
    /// Calculates the position and returns it as a string coordinate.
    /// </summary>
    /// <param name="moveWorker">The MoveWorker containing the board with the position to calculate.</param>
    /// <param name="pivotPosition">The final position will be calculated relative to the pivotPosition</param>
    /// 
    /// <returns>The calculated position as a string.</returns>
    /// 
    public string? GetPosition(MoveWorker moveWorker, string pivotPosition)
    {
        Tuple<int, int>? finalPosition = GetPositionTuple(moveWorker, pivotPosition);
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

    public Tuple<int, int>? GetPositionTuple(MoveWorker moveWorker, string pivotPosition)
    {
        Tuple<int, int>? pivotPositionTuple = moveWorker.Board.ParseCoordinate(pivotPosition);
        if (pivotPositionTuple == null) return null;
        return Tuple.Create(pivotPositionTuple.Item1 + _relativePosition.Item1, pivotPositionTuple.Item2 + _relativePosition.Item2);
    }
}

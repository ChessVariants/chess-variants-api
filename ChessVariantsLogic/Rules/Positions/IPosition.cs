using DataAccess.MongoDB.Models;

namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents a position on the board that can be calculated at runtime.
/// </summary>
public interface IPosition
{
    /// <summary>
    /// Calculates the position and returns it as a string coordinate.
    /// </summary>
    /// <param name="moveWorker">MoveWorker is needed to calculate the final position.</param>
    /// <param name="pivotPosition">The pivotPosition is needed when a relative position is to be calculated.</param>
    /// 
    /// <returns>The calculated position as a string.</returns>
    /// 
    public string? GetPosition(MoveWorker moveWorker, string moveCoordinates);
    /// <summary>
    /// Calculates the position and returns it as a tuple coordinate.
    /// </summary>
    /// <param name="moveWorker">MoveWorker is needed to calculate the final position.</param>
    /// <param name="pivotPosition">The pivotPosition is needed when a relative position is to be calculated.</param>
    /// 
    /// <returns>The calculated position as a tuple of ints.</returns>
    /// 
    public Tuple<int, int>? GetPositionTuple(MoveWorker moveWorker, string pivotPosition);

    public static IPosition? ConstructFromModel(Position positionRec)
    {
        if (positionRec.PositionAbsolute != null)
        {
            return new PositionAbsolute(positionRec.PositionAbsolute.Coordinate);
        }
        else if (positionRec.PositionRelative!= null)
        {
            return new PositionRelative(positionRec.PositionRelative.Y, positionRec.PositionRelative.X, positionRec.PositionRelative.To ? RelativeTo.TO : RelativeTo.FROM);
        }
        return null;
    }
}

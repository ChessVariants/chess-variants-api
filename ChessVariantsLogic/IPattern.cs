namespace ChessVariantsLogic;

/// <summary>
/// An interface for different types of movement patterns.
/// </summary>
public interface IPattern
{

    /// <summary>
    /// Gets the direction in the X-axis.
    /// </summary>
    /// <returns>the direction in the X-axis as an integer.</returns>
    int GetXDir();

    /// <summary>
    /// Gets the direction in the Y-axis.
    /// </summary>
    /// <returns>the direction in the Y-axis as an integer.</returns>
    int GetYDir();

    /// <summary>
    /// Gets the minimum length that this pattern requires to be travelled.
    /// </summary>
    /// <returns>the minimuim length as an integer.</returns>
    int GetMinLength();

        /// <summary>
    /// Gets the maximum length that this pattern allows to be travelled.
    /// </summary>
    /// <returns>the maximum length as an integer.</returns>
    int GetMaxLength();
}
using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;
/// <summary>
/// Represents a special move that can be performed on the board. (examples: castling, en passant, double pawn move...).
/// _actions is a list of actions that will be performed when the Move is performed.
/// _fromTo is a pair of coordinates (e.g. "e2e4) that represent which piece performs the move and where to click to perform the move respectively.
/// </summary>
public class Move
{
    private readonly List<Action> _actions;
    public readonly string From;
    public readonly string To;

    public string FromTo => From + To;
    public readonly PieceClassifier PieceClassifier;

    /// <summary>
    /// Constructor that takes a list of actions, a string fromTo and a PieceClassifier.
    /// </summary>
    /// <param name="actions">The list of actions that the move performs.</param>
    /// <param name="fromTo">A pair of coordinates, the position of the performing piece and where it ends up.</param>
    /// <param name="pieceClassifier">The PieceClassifier of the piece performing the move.</param>
    public Move(List<Action> actions, string fromTo, PieceClassifier pieceClassifier)
    {
        _actions = actions;
        var fromToTuple = MoveWorker.ParseMove(fromTo);
        if (fromToTuple == null) throw new ArgumentException("Move needs to contain proper fromTo coordinate, supplied fromTo coordinate: " + fromTo);
        (From, To) = fromToTuple;

        PieceClassifier = pieceClassifier;
    }

    /// <summary>
    /// Constructs a standard move.
    /// </summary>
    /// <param name="fromTo">A pair of coordinates, the position of the piece to be moved and where it ends up.</param>
    /// <param name="pieceClassifier">The classifier of the piece performing the move.</param>
    public Move(string fromTo, PieceClassifier pieceClassifier) : this(new List<Action>() { new ActionMovePiece(fromTo) }, fromTo, pieceClassifier)
    {
    }


    public List<Action> GetActions()
    {
        return _actions;
    }

    public void AddAction(Action action)
    {
        _actions.Add(action);
    }

}

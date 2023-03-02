namespace ChessVariantsLogic.Predicates;

/// <summary>
/// This predicates evaluates a rule which has to hold for every possible move from the current board state.
/// </summary>
public class ForEvery : IPredicate
{
    private readonly IPredicate _rule;
    private readonly Player _player;

    public ForEvery(IPredicate rule, Player player)
    {
        _rule = rule;
        _player = player;
    }

    /// <summary>
    /// Evaluates to true if the internal rule holds for every possible move in the current board state for the internal player, otherwise false.
    /// </summary>
    /// <param name="thisBoard">The current board state</param>
    /// <param name="_">Irrelevant since this method looks at every possible future board, not just one</param>
    /// <returns>True if the internal rule holds for every possible move in the current board state for the internal player, otherwise false.</returns>
    public bool Evaluate(Chessboard thisBoard, Chessboard _)
    {
        var possibleMoves = thisBoard.GetAllMoves(_player);
        foreach (var move in possibleMoves)
        {
            var nextBoard = thisBoard.CopyBoard();
            nextBoard.Move(move);
            bool ruleSatisfied = _rule.Evaluate(thisBoard, nextBoard);
            if (!ruleSatisfied)
            {
                return false;
            }
        }
        return true;

    }
}
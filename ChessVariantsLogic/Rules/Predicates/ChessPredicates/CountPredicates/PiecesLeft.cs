using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate determines if how many pieces of a certain type are left in the game
/// and compares it with the supplied <see cref="Comparator"/> to evaluate to either
/// true or false.
/// </summary>
public class PiecesLeft : CountPredicate
{
    [JsonProperty]
    private readonly BoardState _state;
    [JsonProperty]
    private readonly string _pieceIdentifier;

    public PiecesLeft(string pieceIdentifier, Comparator comparator, int compareValue, BoardState state) : base(comparator, compareValue)
    {
        _state = state;
        _pieceIdentifier = pieceIdentifier;
    }


    /// <summary>
    /// Evaluates the boolean value of how many pieces left there are compared to the internal <see cref="Comparator"/>.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>The boolean value of how many pieces left there are compared to the internal <see cref="Comparator"/>.</returns>
    public override bool Evaluate(BoardTransition transition)
    {
        var board = _state == BoardState.THIS ? transition.ThisState : transition.NextState;
        int piecesLeft = Utils.FindPiecesOfType(board, _pieceIdentifier).Count();
        return CompareValue(piecesLeft);
    }

}



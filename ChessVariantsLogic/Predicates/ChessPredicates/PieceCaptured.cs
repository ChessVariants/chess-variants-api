using ChessVariantsLogic;

namespace ChessVariantsLogic.Predicates;

public class PieceCaptured : IPredicate
{
    private readonly string _pieceIdentifier;

    public PieceCaptured(string pieceIdentifier)
    {
        _pieceIdentifier = pieceIdentifier;
    }


    public bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        int amountOfPieces = Utils.FindPiecesOfType(thisBoardState, _pieceIdentifier).Count();
        int amountOfPiecesNextState = Utils.FindPiecesOfType(nextBoardState, _pieceIdentifier).Count();
        int diff = amountOfPiecesNextState - amountOfPieces;
        return diff > 0;
    }

}
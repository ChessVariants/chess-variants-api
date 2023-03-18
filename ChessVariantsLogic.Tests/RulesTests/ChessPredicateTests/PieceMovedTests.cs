using Xunit;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Tests;

public class PieceMovedTests
{
    MoveWorker board;
    BoardTransition boardTransition;
    string fromStr;
    string toStr;

    public PieceMovedTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        fromStr = "e2";
        toStr = "e3";
        Move move = new Move(fromStr + toStr, PieceClassifier.WHITE);
        boardTransition = new BoardTransition(board, move);
    }

    [Fact]
    public void PawnMoved_ShouldReturnTrue()
    {
        IPredicate pawnMoved = new PieceMoved(Constants.WhitePawnIdentifier);
        Assert.True(pawnMoved.Evaluate(boardTransition));
    }

    [Fact]
    public void QueenMoved_ShouldReturnFalse()
    {
        IPredicate queenMoved = new PieceMoved(Constants.WhiteQueenIdentifier);
        Assert.False(queenMoved.Evaluate(boardTransition));
    }
}
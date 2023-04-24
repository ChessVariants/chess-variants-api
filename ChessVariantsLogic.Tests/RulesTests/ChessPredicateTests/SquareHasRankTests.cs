using Xunit;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Tests;

public class SquareHasRankTests {
    private readonly MoveWorker board;
    
    private readonly BoardTransition transition;

    public SquareHasRankTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        Move move = new Move("e2e3", Piece.WhitePawn());
        transition = new BoardTransition(board, move);
    }

    [Fact]
    public void PawnHasRankThree_ShouldReturnTrue()
    {
        IPredicate pawnHasRankThree = new SquareHasRank(new PositionRelative(0, 0, RelativeTo.TO), 3);
        Assert.True(pawnHasRankThree.Evaluate(transition));
    }

    [Fact]
    public void PawnHasRankThree_ShouldReturnFalse()
    {
        IPredicate pawnHasRankThree = new SquareHasRank(new PositionRelative(0, 0, RelativeTo.FROM), 3);
        Assert.False(pawnHasRankThree.Evaluate(transition));
    }

}
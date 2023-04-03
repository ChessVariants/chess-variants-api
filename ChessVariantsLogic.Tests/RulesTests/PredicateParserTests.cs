
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules;
using System;
using Xunit;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using Newtonsoft.Json;

namespace ChessVariantsLogic.Tests.RulesTests;
public class PredicateParserTests
{

    IPredicate constTrue;
    IPredicate constFalse;
    MoveWorker board;
    BoardTransition transition;
    PredicateParser pp;

    public PredicateParserTests()
    {
        pp = new PredicateParser();
        constTrue = new Const(true);
        constFalse = new Const(false);
        board = new MoveWorker(new Chessboard(8));
        transition = new BoardTransition(board, new Move("e2e3", Piece.WhitePawn()));
    }

    [Fact]
    public void ConstTrue_shouldBeEqual()
    {
        var pred = new Const(true);

        Assert.Equal(JsonConvert.SerializeObject(pred), JsonConvert.SerializeObject(pp.ParsePredicate("true")));
    }
    [Fact]
    public void ConstFalse_shouldBeEqual()
    {
        var pred = new Const(false);

        Assert.Equal(JsonConvert.SerializeObject(pred), JsonConvert.SerializeObject(pp.ParsePredicate("false")));
    }
    [Fact]
    public void And_shouldBeEqual()
    {
        IPredicate fals = new Const(false);
        IPredicate tru = new Const(true);

        Assert.Equal(JsonConvert.SerializeObject((fals & fals)), JsonConvert.SerializeObject(pp.ParsePredicate("AND(false, false)")));
        Assert.Equal(JsonConvert.SerializeObject((fals & tru)), JsonConvert.SerializeObject(pp.ParsePredicate("AND(false, true)")));
        Assert.Equal(JsonConvert.SerializeObject((tru & fals)), JsonConvert.SerializeObject(pp.ParsePredicate("AND(true, false)")));
        Assert.Equal(JsonConvert.SerializeObject((tru & tru)), JsonConvert.SerializeObject(pp.ParsePredicate("AND(true, true)")));
    }
    [Fact]
    public void And_shouldBeEqualNested()
    {
        IPredicate fals = new Const(false);
        IPredicate tru = new Const(true);

        Assert.Equal(JsonConvert.SerializeObject((tru & (fals | (tru & (fals | tru))))), JsonConvert.SerializeObject(pp.ParsePredicate("AND(true, OR(false, AND(true, OR(false, true))))")));
    }
    [Fact]
    public void PawnMoved_shouldBeEqual()
    {
        IPredicate pawnMoved = new PieceMoved("PA");

        Assert.Equal(JsonConvert.SerializeObject(pawnMoved), JsonConvert.SerializeObject(pp.ParsePredicate("move_pred(this, name, PA)")));
    }
    [Fact]
    public void StandardChessMoveRule_shouldBeEqual()
    {
        IPredicate moveRule = new Operator(OperatorType.NOT, new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(pp.ParsePredicate("NOT(piece_pred(attacked, KI, next))")));
    }

    [Fact]
    public void StandardChessMoveRuleScript_shouldBeEqual()
    {
        IPredicate moveRule = new Operator(OperatorType.NOT, new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(pp.ParseCode("" +
            "x=bi\n" +
            "white_king = KI\n\n" +
            "white_king_checked_next_turn = piece_pred(attacked, white_king, next)\n" +
            "white_move_rule = NOT(white_king_checked_next_turn)\n" +
            "return = white_move_rule\n")));
    }
}

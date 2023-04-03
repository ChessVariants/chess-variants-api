
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

        Assert.Equal(JsonConvert.SerializeObject(pawnMoved), JsonConvert.SerializeObject(pp.ParsePredicate("move_pred(this_move, piece_moved, PA)")));
    }
    [Fact]
    public void StandardChessMoveRule_shouldBeEqual()
    {
        IPredicate moveRule = new Operator(OperatorType.NOT, new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(pp.ParsePredicate("NOT(piece_pred(next_state, attacked, KI))")));
    }

    [Fact]
    public void StandardChessMoveRuleScript_shouldBeEqual()
    {
        IPredicate moveRule = new Operator(OperatorType.NOT, new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(pp.ParseCode("" +
            "x=bi\n" +
            "white_king = KI\n\n" +
            "white_king_checked_next_turn = piece_pred(next_state, attacked, white_king)\n" +
            "white_move_rule = NOT(white_king_checked_next_turn)\n" +
            "return = white_move_rule\n")));
    }

    [Fact]
    public void AntiChessMoveRuleScript_shouldBeEqual()
    {
        IPredicate moveRule = new Operator(new Attacked(BoardState.THIS, "BLACK"), OperatorType.IMPLIES, new PieceCaptured("BLACK"));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(pp.ParseCode("" +
            "black_attacked = piece_pred(this_state, attacked, BLACK)\n" +
            "black_captured = move_pred(this_move, captured, BLACK)\n" +
            "return = IMPLIES(black_attacked, black_captured)\n")));
    }

    [Fact]
    public void DuckChessMoveRuleScript_shouldBeEqual()
    {
        IPredicate lastMoveWasDuck = new PieceMoved(Constants.DuckIdentifier, MoveState.LAST);
        IPredicate lastMoveWasWhite = new PieceMoved("WHITE", MoveState.LAST);

        IPredicate thisMoveWasDuckMove = new PieceMoved(Constants.DuckIdentifier, MoveState.THIS);

        IPredicate firstMove = new FirstMove();

        IPredicate moveRule = ((thisMoveWasDuckMove & lastMoveWasWhite) | (!thisMoveWasDuckMove & (lastMoveWasDuck | firstMove)));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(pp.ParseCode("" +
            "last_move_duck = move_pred(last_move, piece_moved, DU)\n" +
            "last_move_white = move_pred(last_move, piece_moved, WHITE)\n" +
            "\n" +
            "this_move_duck = move_pred(this_move, piece_moved, DU)\n" +
            "\n" +
            "first_m = move_pred(this_move, first_move)\n" +
            "\n" +
            "move_rule = OR(AND(this_move_duck,last_move_white), AND(NOT(this_move_duck), OR(last_move_duck, first_m)))\n" +
            "\r\n" +
            "return = move_rule\n")));
    }
}

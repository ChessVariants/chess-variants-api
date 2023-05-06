
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules;
using System;
using Xunit;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChessVariantsLogic.Tests.RulesTests;
public class PredicateParserTests
{

    IPredicate constTrue;
    IPredicate constFalse;
    MoveWorker board;
    BoardTransition transition;

    public PredicateParserTests()
    {
        constTrue = new Const(true);
        constFalse = new Const(false);
        board = new MoveWorker(new Chessboard(8));
        transition = new BoardTransition(board, new Move("e2e3", Piece.WhitePawn()));
    }

    [Fact]
    public void ConstTrue_ShouldBeEqual()
    {
        var pred = new Const(true);

        Assert.Equal(JsonConvert.SerializeObject(pred), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("true")));
    }
    [Fact]
    public void ConstFalse_ShouldBeEqual()
    {
        var pred = new Const(false);

        Assert.Equal(JsonConvert.SerializeObject(pred), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("false")));
    }
    [Fact]
    public void And_ShouldBeEqual()
    {
        IPredicate fals = new Const(false);
        IPredicate tru = new Const(true);

        Assert.Equal(JsonConvert.SerializeObject((fals & fals)), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("AND(false, false)")));
        Assert.Equal(JsonConvert.SerializeObject((fals & tru)), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("AND(false, true)")));
        Assert.Equal(JsonConvert.SerializeObject((tru & fals)), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("AND(true, false)")));
        Assert.Equal(JsonConvert.SerializeObject((tru & tru)), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("AND(true, true)")));
    }
    [Fact]
    public void And_ShouldBeEqualNested()
    {
        IPredicate fals = new Const(false);
        IPredicate tru = new Const(true);

        Assert.Equal(JsonConvert.SerializeObject((tru & (fals | (tru & (fals | tru))))), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("AND(true, OR(false, AND(true, OR(false, true))))")));
    }
    [Fact]
    public void PawnMoved_ShouldBeEqual()
    {
        IPredicate pawnMoved = new PieceMoved("PA");

        Assert.Equal(JsonConvert.SerializeObject(pawnMoved), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("move_piece_is(this_move, PA)")));
    }
    [Fact]
    public void StandardChessMoveRule_ShouldBeEqual()
    {
        IPredicate moveRule = new Operator(OperatorType.NOT, new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("NOT(piece_attacked(next_state, KI))")));
    }

    [Fact]
    public void CountPred_ShouldBeEqual()
    {
        IPredicate countPred = new PiecesLeft(Constants.WhiteKingIdentifier, Comparator.EQUALS, 0, BoardState.NEXT);
        Assert.Equal(JsonConvert.SerializeObject(countPred), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("count_pieces_with_id(next_state, KI, equals, 0)")));
    }
    [Fact]
    public void FilePred_ShouldBeEqual()
    {
        IPredicate countPred = new SquareHasFile(new PositionRelative(0, 1, RelativeTo.FROM), 1);
        Assert.Equal(JsonConvert.SerializeObject(countPred), JsonConvert.SerializeObject(PredicateParser.ParsePredicate("square_has_file(relative(0, 1, from), 1)")));
    }

    [Fact]
    public void StandardChessMoveRuleScript_ShouldBeEqual()
    {
        IPredicate moveRule = new Operator(OperatorType.NOT, new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(PredicateParser.ParseCode("" +
            "x=bi\n" +
            "white_king = KI\n\n" +
            "white_king_checked_next_turn = piece_attacked(next_state, white_king)\n" +
            "white_move_rule = NOT(white_king_checked_next_turn)\n" +
            "return = white_move_rule\n")));
    }

    [Fact]
    public void PawnMoveScript_ShouldBeEqual()
    {
        string script = "this_position = relative(0, 0, from)\n" +
            "position_one = relative(0, -1, from)\n" +
            "position_two = relative(0, -2, from)\n" +
            "\n" +
            "has_moved = square_has_moved(this_state, this_position)\n" +
            "target_square_empty_one = square_has_piece(this_state, position_one, --)\n" +
            "target_square_empty_two = square_has_piece(this_state, position_two, --)\n" +
            "\n" +
            "final_pred = !has_moved && target_square_empty_one && target_square_empty_two\n" +
            "\n" +
            "return = final_pred";

        IPredicate test = PredicateParser.ParseCode(script);
        test.ToString();
    }


    [Fact]
    public void AntiChessMoveRuleScript_ShouldBeEqual()
    {
        IPredicate moveRule = new Operator(new Attacked(BoardState.THIS, "BLACK"), OperatorType.IMPLIES, new PieceCaptured("BLACK"));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(PredicateParser.ParseCode("" +
            "black_attacked = piece_attacked(this_state, BLACK)\n" +
            "black_captured = move_captured(this_move, BLACK)\n" +
            "return = IMPLIES(black_attacked, black_captured)\n")));
    }

    [Fact]
    public void DuckChessMoveRuleScript_ShouldBeEqual()
    {
        IPredicate lastMoveWasDuck = new PieceMoved(Constants.DuckIdentifier, MoveState.LAST);
        IPredicate lastMoveWasWhite = new PieceMoved("WHITE", MoveState.LAST);

        IPredicate thisMoveWasDuckMove = new PieceMoved(Constants.DuckIdentifier, MoveState.THIS);

        IPredicate firstMove = new FirstMove();

        IPredicate moveRule = ((thisMoveWasDuckMove & lastMoveWasWhite) | (!thisMoveWasDuckMove & (lastMoveWasDuck | firstMove)));

        Assert.Equal(JsonConvert.SerializeObject(moveRule), JsonConvert.SerializeObject(PredicateParser.ParseCode("" +
            "last_move_duck = move_piece_is(last_move, DU)\n" +
            "last_move_white = move_piece_is(last_move, WHITE)\n" +
            "\n" +
            "this_move_duck = move_piece_is(this_move, DU)\n" +
            "\n" +
            "first_m = move_first(this_move)\n" +
            "\n" +
            "move_rule_new = this_move_duck && last_move_white || !this_move_duck && [last_move_duck || first_m]\n" +
            "move_rule = OR(AND(this_move_duck,last_move_white), AND(NOT(this_move_duck), OR(last_move_duck, first_m)))\n" +
            "\r\n" +
            "return = move_rule_new\n")));
    }
    [Fact]
    public void SyntaxNames_ShouldThrowException()
    {
        Assert.Throws<PrediChessException>(() => PredicateParser.ParseCode("this_move = move_piece_is(this_move, DU)\n" +
            "return = this_move\n"));
    }
    [Fact]
    public void ShuntingYard_()
    {
        List<string> input = PredicateParser.ShuntingYard("!(test && b) || (c && d) == f");
        string output = PredicateParser.ConvertToExpression(input);
        Assert.True(output.Length > 0);
    }
}

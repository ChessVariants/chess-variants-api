namespace ChessVariantsLogic;

using Predicates;

public static class GameFactory
{

    private static OperatorType AND = OperatorType.AND;
    private static OperatorType OR = OperatorType.OR;
    private static OperatorType IMPLIES = OperatorType.IMPLIES;
    private static OperatorType XOR = OperatorType.XOR;
    private static OperatorType EQUALS = OperatorType.EQUALS;
    private static OperatorType NOT = OperatorType.NOT;

    public static Game StandardChess()
    {
        IPredicate blackKingCheckedThisTurn = new Attacked(BoardState.THIS, Constants.BlackKingIdentifier);
        IPredicate blackKingCheckedNextTurn = new Attacked(BoardState.NEXT, Constants.BlackKingIdentifier);
        IPredicate whiteKingCheckedThisTurn = new Attacked(BoardState.THIS, Constants.WhiteKingIdentifier);
        IPredicate whiteKingCheckedNextTurn = new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier);

        IPredicate whiteWinRule = new Operator(blackKingCheckedThisTurn, AND, blackKingCheckedNextTurn);
        IPredicate whiteMoveRule = new Operator(NOT, whiteKingCheckedNextTurn);
        IPredicate blackWinRule = new Operator(whiteKingCheckedThisTurn, AND, whiteKingCheckedNextTurn);
        IPredicate blackMoveRule = new Operator(NOT, blackKingCheckedNextTurn);

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, whiteWinRule);
        RuleSet rulesBlack = new RuleSet(blackMoveRule, blackWinRule);

        return new Game(Chessboard.StandardChessboard(), Player.White, 1, rulesWhite, rulesBlack);
    }

    public static Game CaptureTheKing()
    {
        IPredicate whiteMoveRule = new Const(true);
        IPredicate whiteWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, Constants.BlackKingIdentifier);

        
        IPredicate blackMoveRule = new Const(true);
        IPredicate blackWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, Constants.WhiteKingIdentifier);

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, whiteWinRule);
        RuleSet rulesBlack = new RuleSet(whiteMoveRule, whiteWinRule);
        
        return new Game(Chessboard.StandardChessboard(), Player.White, 1, rulesWhite, rulesBlack);
    }

    public static Game AntiChess()
    {
        
        IPredicate whiteMoveRule = new Operator(new Attacked(BoardState.THIS, "ANY_BLACK"), IMPLIES, new PieceCaptured("ANY_BLACK"));
        IPredicate whiteWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, "ANY_WHITE");
        
        IPredicate blackMoveRule = new Operator(new Attacked(BoardState.THIS, "ANY_WHITE"), IMPLIES, new PieceCaptured("ANY_WHITE"));
        IPredicate blackWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, "ANY_BLACK");

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, whiteWinRule);
        RuleSet rulesBlack = new RuleSet(blackMoveRule, blackWinRule);

        return new Game(Chessboard.StandardChessboard(), Player.White, 1, rulesWhite, rulesBlack);
    }
}
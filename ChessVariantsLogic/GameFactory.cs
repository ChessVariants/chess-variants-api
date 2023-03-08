namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Moves.Actions;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// Factory with predetermined rules for the known variants. WIP
/// </summary>
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

        IPredicate blackKingCheckedThisAndNextTurn = new Operator(blackKingCheckedThisTurn, AND, blackKingCheckedNextTurn);
        IPredicate whiteKingCheckedThisAndNextTurn = new Operator(whiteKingCheckedThisTurn, AND, whiteKingCheckedNextTurn);
        
        IPredicate whiteWinRule = new ForEvery(blackKingCheckedThisAndNextTurn, Player.Black);  
        IPredicate blackWinRule = new ForEvery(whiteKingCheckedThisAndNextTurn, Player.White);

        IPredicate whiteMoveRule = new Operator(NOT, whiteKingCheckedNextTurn);        
        IPredicate blackMoveRule = new Operator(NOT, blackKingCheckedNextTurn);

        
        IPredicate blackPawnRight = new PieceAt(Constants.BlackPawnIdentifier, Tuple.Create(1, 0), BoardState.THIS, PositionType.RELATIVE);
        IEnumerable<IAction> actions = new List<IAction> { new ActionMovePieceRelative(Tuple.Create(1, 1)) };


        ISet<MoveData> movesWhite = new HashSet<MoveData>
        {
            MoveData.CastleMove(Player.White, true),
            MoveData.CastleMove(Player.White, false),
            MoveData.PawnDoubleMove(Player.White),
            MoveData.EnPassantMove(Player.White, true),
            MoveData.EnPassantMove(Player.White, false),
        };


        ISet<MoveData> movesBlack = new HashSet<MoveData>
        {
            MoveData.CastleMove(Player.Black, true),
            MoveData.CastleMove(Player.Black, false),
            MoveData.PawnDoubleMove(Player.Black),
            MoveData.EnPassantMove(Player.Black, true),
            MoveData.EnPassantMove(Player.Black, false),
        };


        RuleSet rulesWhite = new RuleSet(whiteMoveRule, whiteWinRule, movesWhite);
        RuleSet rulesBlack = new RuleSet(blackMoveRule, blackWinRule, movesBlack);



        return new Game(new MoveWorker(Chessboard.StandardChessboard()), Player.White, 1, rulesWhite, rulesBlack);
    }

    public static Game CaptureTheKing()
    {
        IPredicate whiteMoveRule = new Const(true);
        IPredicate whiteWinRule = new PiecesLeft(Constants.BlackKingIdentifier, Comparator.EQUALS, 0, BoardState.THIS);

        
        IPredicate blackMoveRule = new Const(true);
        IPredicate blackWinRule = new PiecesLeft(Constants.WhiteKingIdentifier, Comparator.EQUALS, 0, BoardState.THIS);

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, whiteWinRule, new HashSet<MoveData>());
        RuleSet rulesBlack = new RuleSet(whiteMoveRule, whiteWinRule, new HashSet<MoveData>());
        
        return new Game(new MoveWorker(Chessboard.StandardChessboard()), Player.White, 1, rulesWhite, rulesBlack);
    }

    public static Game AntiChess()
    {
        
        IPredicate whiteMoveRule = new Operator(new Attacked(BoardState.THIS, "ANY_BLACK"), IMPLIES, new PieceCaptured("ANY_BLACK"));
        IPredicate whiteWinRule = new PiecesLeft("ANY_WHITE", Comparator.EQUALS, 0, BoardState.THIS);
        
        IPredicate blackMoveRule = new Operator(new Attacked(BoardState.THIS, "ANY_WHITE"), IMPLIES, new PieceCaptured("ANY_WHITE"));
        IPredicate blackWinRule = new PiecesLeft("ANY_BLACK", Comparator.EQUALS, 0, BoardState.THIS);

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, whiteWinRule, new HashSet<MoveData>());
        RuleSet rulesBlack = new RuleSet(blackMoveRule, blackWinRule, new HashSet<MoveData>());

        return new Game(new MoveWorker(Chessboard.StandardChessboard()), Player.White, 1, rulesWhite, rulesBlack);
    }
}
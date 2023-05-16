using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;
public class AIPlayer
{
    private readonly IMoveFinder _moveFinder;
    public Player PlayingAs { get; set; }

    public AIPlayer(IMoveFinder moveFinder, Player player)
    {
        _moveFinder = moveFinder;
        PlayingAs = player;
    }

    public Move SearchMove(Game game, int depth=2, int tradeDepth=3)
    {
        return _moveFinder.FindBestMove(depth, tradeDepth, game, PlayingAs, ScoreVariant.RegularChess);
    }

    public string GetMostValuablePieceIdentifier(ISet<string> promotablePieces, Player forPlayer)
    {
        return _moveFinder.GetPieceValue().GetHighestValuePieceIdentifier(promotablePieces, forPlayer);
    }
}

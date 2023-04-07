using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;
public class AIPlayer
{
    private readonly IMoveFinder _moveFinder;
    public readonly Player PlayingAs;

    public AIPlayer(IMoveFinder moveFinder, Player player)
    {
        _moveFinder = moveFinder;
        PlayingAs = player;
    }

    public Move SearchMove(Game game, int depth=3)
    {
        return _moveFinder.findBestMove(depth, game, PlayingAs);
    }
}

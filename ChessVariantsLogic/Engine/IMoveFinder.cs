using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;
public interface IMoveFinder
{
    public Move findBestMove(int depth, Game game, Player player);
}

using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;
public interface IMoveFinder
{

    public Move FindBestMove(int depth, int tradeDepth, Game game, Player player, ScoreVariant scoreVariant);
    public PieceValue GetPieceValue();
}

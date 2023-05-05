using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Engine;
public interface IMoveFinder
{

    public Move FindBestMove(int depth, Game game, Player player, ScoreVariant scoreVariant);
    public PieceValue GetPieceValue();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Engine;
public static class AIFactory
{
    public static AIPlayer NegaMaxAI(Player player)
    {
        var pieceValues = new PieceValue(Piece.AllStandardPieces());
        var negaMax = new NegaMax(pieceValues);
        return new AIPlayer(negaMax, player);
    }
}

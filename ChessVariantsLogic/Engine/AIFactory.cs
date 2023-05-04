﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Engine;
public static class AIFactory
{
    public static AIPlayer NegaMaxAI(Player player)
    {
        var pieces = Piece.AllStandardPieces();
        var chessboard = new Chessboard(8,8);
        var negaMax = new NegaMax(pieces, chessboard);
        return new AIPlayer(negaMax, player);
    }
}

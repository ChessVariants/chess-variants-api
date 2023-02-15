namespace ChessVariantsLogic;

public class ChessBoard
{
    private char[,] Board { get; set; }

    public static Dictionary<String, (int,int)> CoorToIndex = new Dictionary<String, (int, int)>()
    {
        {"a1", (7,0) },
        {"a2", (7,1) },
        {"a3", (7,2) },
        {"a4", (7,3) },
        {"a5", (7,4) },
        {"a6", (7,5) },
        {"a7", (7,6) },
        {"a8", (7,7) },

        {"b1", (6,0) },
        {"b2", (6,1) },
        {"b3", (6,2) },
        {"b4", (6,3) },
        {"b5", (6,4) },
        {"b6", (6,5) },
        {"b7", (6,6) },
        {"b8", (6,7) },

        {"c1", (5,0) },
        {"c2", (5,1) },
        {"c3", (5,2) },
        {"c4", (5,3) },
        {"c5", (5,4) },
        {"c6", (5,5) },
        {"c7", (5,6) },
        {"c8", (5,7) },

        {"d1", (4,0) },
        {"d2", (4,1) },
        {"d3", (4,2) },
        {"d4", (4,3) },
        {"d5", (4,4) },
        {"d6", (4,5) },
        {"d7", (4,6) },
        {"d8", (4,7) },

        {"e1", (3,0) },
        {"e2", (3,1) },
        {"e3", (3,2) },
        {"e4", (3,3) },
        {"e5", (3,4) },
        {"e6", (3,5) },
        {"e7", (3,6) },
        {"e8", (3,7) },

        {"f1", (2,0) },
        {"f2", (2,1) },
        {"f3", (2,2) },
        {"f4", (2,3) },
        {"f5", (2,4) },
        {"f6", (2,5) },
        {"f7", (2,6) },
        {"f8", (2,7) },

        {"g1", (1,0) },
        {"g2", (1,1) },
        {"g3", (1,2) },
        {"g4", (1,3) },
        {"g5", (1,4) },
        {"g6", (1,5) },
        {"g7", (1,6) },
        {"g8", (1,7) },

        {"h1", (0,0) },
        {"h2", (0,1) },
        {"h3", (0,2) },
        {"h4", (0,3) },
        {"h5", (0,4) },
        {"h6", (0,5) },
        {"h7", (0,6) },
        {"h8", (0,7) },

    };

    public ChessBoard(int rows, int cols)
    {
        Board = new char[rows, cols] ;
    }

    public static ChessBoard StandardChessBoard()
    {
        var chessBoard = new ChessBoard(8, 8);

        // small letters black pieces, capitalized letters white pieces
        chessBoard.Board[0, 0] = 'r';
        chessBoard.Board[0, 1] = 'n';
        chessBoard.Board[0, 2] = 'b';
        chessBoard.Board[0, 3] = 'q';
        chessBoard.Board[0, 4] = 'k';
        chessBoard.Board[0, 5] = 'b';
        chessBoard.Board[0, 6] = 'n';
        chessBoard.Board[0, 7] = 'r';

        chessBoard.Board[7, 0] = 'R';
        chessBoard.Board[7, 1] = 'N';
        chessBoard.Board[7, 2] = 'B';
        chessBoard.Board[7, 3] = 'Q';
        chessBoard.Board[7, 4] = 'K';
        chessBoard.Board[7, 5] = 'B';
        chessBoard.Board[7, 6] = 'N';
        chessBoard.Board[7, 7] = 'R';

        // pawns
        for (int i = 0; i < 8; i++)
        {
            chessBoard.Board[1, i] = 'p';
            chessBoard.Board[6, i] = 'P';
        }

        return chessBoard;
    }

}

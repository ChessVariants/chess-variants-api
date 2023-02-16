namespace ChessVariantsLogic;

public class Chessboard
{
    public char[,] Board { get; set; }

    // TODO: make dynamic to fit boardsize
    public static Dictionary<String, (int, int)> CoorToIndex = new Dictionary<String, (int, int)>()
    {
        {"h8", (0,7) },
        {"h7", (1,7) },
        {"h6", (2,7) },
        {"h5", (3,7) },
        {"h4", (4,7) },
        {"h3", (5,7) },
        {"h2", (6,7) },
        {"h1", (7,7) },

        {"g8", (0,6) },
        {"g7", (1,6) },
        {"g6", (2,6) },
        {"g5", (3,6) },
        {"g4", (4,6) },
        {"g3", (5,6) },
        {"g2", (6,6) },
        {"g1", (7,6) },

        {"f8", (0,5) },
        {"f7", (1,5) },
        {"f6", (2,5) },
        {"f5", (3,5) },
        {"f4", (4,5) },
        {"f3", (5,5) },
        {"f2", (6,5) },
        {"f1", (7,5) },

        {"e8", (0,4) },
        {"e7", (1,4) },
        {"e6", (2,4) },
        {"e5", (3,4) },
        {"e4", (4,4) },
        {"e3", (5,4) },
        {"e2", (6,4) },
        {"e1", (7,4) },

        {"d8", (0,3) },
        {"d7", (1,3) },
        {"d6", (2,3) },
        {"d5", (3,3) },
        {"d4", (4,3) },
        {"d3", (5,3) },
        {"d2", (6,3) },
        {"d1", (7,3) },

        {"c8", (0,2) },
        {"c7", (1,2) },
        {"c6", (2,2) },
        {"c5", (3,2) },
        {"c4", (4,2) },
        {"c3", (5,2) },
        {"c2", (6,2) },
        {"c1", (7,2) },

        {"b8", (0,1) },
        {"b7", (1,1) },
        {"b6", (2,1) },
        {"b5", (3,1) },
        {"b4", (4,1) },
        {"b3", (5,1) },
        {"b2", (6,1) },
        {"b1", (7,1) },

        {"a8", (0,0) },
        {"a7", (1,0) },
        {"a6", (2,0) },
        {"a5", (3,0) },
        {"a4", (4,0) },
        {"a3", (5,0) },
        {"a2", (6,0) },
        {"a1", (7,0) },

    };

    // Updates chessboard
    // @param move is the string representation of a move
    public void MakeMove(String move)
    {
        String from, to;
        (from, to) = parseMove(move);

        (int, int) fromIndex = CoorToIndex[from];
        (int, int) toIndex = CoorToIndex[to];
        
        char piece = Board[fromIndex.Item1, fromIndex.Item2];
        
        Board[toIndex.Item1, toIndex.Item2] = piece;
        Board[fromIndex.Item1, fromIndex.Item2] = '-';
    }

    // Splits the string move into the substrings representing the "from" square and "to" square 
    private (String, String) parseMove(String move)
    {
        String from = "", to = "";
        switch (move.Length)
        {
            case 4 : from = move.Substring(0,2); to = move.Substring(2,2); break;
            case 5 :
            {
                if(char.IsNumber(move[2]))
                {
                    from = move.Substring(0,3);
                    to = move.Substring(3,2);
                }
                else
                {
                    from = move.Substring(0,2);
                    to = move.Substring(2,3);
                }
                break;
            }
            case 6 : from = move.Substring(0,3); to = move.Substring(3,3); break;
        }
        Console.WriteLine("from: " + from + ", to: " + to);
        return (from, to);
    }

    public Chessboard(int rows, int cols)
    {
        Board = new char[rows, cols] ;
    }

    public static Chessboard StandardChessboard()
    {
        var chessboard = new Chessboard(8, 8);

        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                chessboard.Board[i,j] = '-';
            }
        }

        // small letters black pieces, capitalized letters white pieces
        chessboard.Board[0, 0] = 'r';
        chessboard.Board[0, 1] = 'n';
        chessboard.Board[0, 2] = 'b';
        chessboard.Board[0, 3] = 'q';
        chessboard.Board[0, 4] = 'k';
        chessboard.Board[0, 5] = 'b';
        chessboard.Board[0, 6] = 'n';
        chessboard.Board[0, 7] = 'r';

        chessboard.Board[7, 0] = 'R';
        chessboard.Board[7, 1] = 'N';
        chessboard.Board[7, 2] = 'B';
        chessboard.Board[7, 3] = 'Q';
        chessboard.Board[7, 4] = 'K';
        chessboard.Board[7, 5] = 'B';
        chessboard.Board[7, 6] = 'N';
        chessboard.Board[7, 7] = 'R';

        // pawns
        for (int i = 0; i < 8; i++)
        {
            chessboard.Board[1, i] = 'p';
            chessboard.Board[6, i] = 'P';
        }

        return chessboard;
    }
    
}

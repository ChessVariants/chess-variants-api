namespace ChessVariantsLogic;

public class ChessBoard
{
    private char[,] Board { get; set; }

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

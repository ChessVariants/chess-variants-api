using ChessVariantsLogic.Export;

namespace ChessVariantsLogic.Editor;

public class BoardEditor
{
    private MoveWorker _mw;     // This holds the imagepaths for all pieces
    private MoveWorker _mwPieceName;   // This holds the piece ID for the db for all pieces.

    private Piece? _piece;

    public BoardEditor()
    {
        _mw = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        _mwPieceName = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
    }

    public void UpdateBoardSize(int row, int col)
    {
        _mw.Board = new Chessboard(row, col);
        _mwPieceName.Board = new Chessboard(row, col);
    }

    public void InsertPiece(string square)
    {
        if (_piece == null)
            return;
        if(_piece.ImagePath.Equals("remove"))
        {
            _mw.RemoveFromBoard(square);
            _mwPieceName.RemoveFromBoard(square);
        }
        else
        {
            _mw.InsertOnBoard(_piece, square);
            _mwPieceName.InsertOnBoard(_piece, square);
        }
    }

    //Parameter "piece" should probably be processed in some way.
    public void SetActivePiece(Piece piece)
    {
        _piece = piece;
    }

    public void ResetStartingPosition()
    {
        _mw.Board = Chessboard.StandardChessboard();
        _mwPieceName.Board = Chessboard.StandardChessboard();
    }

    public void ClearBoard()
    {
        int row = _mw.Board.Rows;
        int col = _mw.Board.Cols;
        _mw.Board = new Chessboard(row, col);
        _mwPieceName.Board = new Chessboard(row, col);
    }

    public void SaveBoardToDb()
    {
        // This should use _mwPieceName to save all the piece IDs.
    }

    public BoardEditorState GetCurrentState() { return EditorExporter.ExportBoardEditorState(_mw); }

}
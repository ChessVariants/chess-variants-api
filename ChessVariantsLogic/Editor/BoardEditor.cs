using ChessVariantsLogic.Export;

namespace ChessVariantsLogic.Editor;

public class BoardEditor
{
    private MoveWorker _mw;     // This holds the imagepaths for all pieces

    private Piece? _piece;

    private bool _remove;

    public BoardEditor()
    {
        _mw = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        _remove = false;
    }

    public void UpdateBoardSize(int row, int col)
    {
        _mw.Board = new Chessboard(row, col);
    }

    public void UpdateSquare(string square)
    {
        if(_remove)
        {
            _mw.RemoveFromBoard(square);
            return;
        }

        if (_piece == null)
            return;
        else
        {
            _mw.InsertOnBoard(_piece, square);
        }
    }

    public void SetActiveRemove() { _remove = true; }

    public void SetActivePiece(Piece piece)
    {
        _remove = false;
        _piece = piece;
    }

    public void ResetStartingPosition()
    {
        _mw.Board = Chessboard.StandardChessboard();
    }

    public void ClearBoard()
    {
        int row = _mw.Board.Rows;
        int col = _mw.Board.Cols;
        _mw.Board = new Chessboard(row, col);
    }

    public void SaveBoardToDb()
    {
        // This should use _mwPieceName to save all the piece IDs.
    }

    public BoardEditorState GetCurrentState() { return EditorExporter.ExportBoardEditorState(_mw); }

}
using ChessVariantsLogic.Export;

namespace ChessVariantsLogic.Editor;

public class BoardEditor
{
    private Chessboard _board;
    private string? _piece;

    public BoardEditor()
    {
        _board = Chessboard.StandardChessboard();
    }

    public void UpdateBoardSize(int row, int col)
    {
        _board = new Chessboard(row, col);
    }

    //Parameter "piece" should probably be processed in some way.
    public void InsertPiece(int row, int col)
    {
        if(_piece != null)
            _board.Insert(_piece, row, col);
    }

    public void RemovePiece(int row, int col) { _board.Remove(row, col); }

    public void SetActivePiece(string piece) { _piece = piece; }

    public void ResetStartingPosition() { _board = Chessboard.StandardChessboard(); }

    public void ClearBoard()
    {
        int row = _board.Rows;
        int col = _board.Cols;
        _board = new Chessboard(row, col);
    }

    public BoardEditorState GetCurrentState() { return EditorExporter.ExportBoardEditorState(_board); }

}
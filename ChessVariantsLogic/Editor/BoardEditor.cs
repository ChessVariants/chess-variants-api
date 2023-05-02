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

    public void InsertPiece(string square)
    {
        if(_piece == null)
            return;
        if(_piece.Equals("remove"))
            _board.Remove(square);
        else
            _board.Insert(_piece, square);
    }

    //Parameter "piece" should probably be processed in some way.
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
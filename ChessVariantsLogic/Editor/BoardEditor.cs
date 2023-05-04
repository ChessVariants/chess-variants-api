using ChessVariantsLogic.Export;

namespace ChessVariantsLogic.Editor;

public class BoardEditor
{
    private MoveWorker _mw;
    private string? _piece;

    public BoardEditor()
    {

        _mw = new MoveWorker(Chessboard.StandardChessboard());
    }

    public void UpdateBoardSize(int row, int col)
    {
        _mw.Board = new Chessboard(row, col);
    }

    public void InsertPiece(string square)
    {
        if(_piece == null)
            return;
        if(_piece.Equals("remove"))
            _mw.Board.Remove(square);
        else
            _mw.Board.Insert(_piece, square);
    }

    //Parameter "piece" should probably be processed in some way.
    public void SetActivePiece(string piece) { _piece = piece; }

    public void ResetStartingPosition() { _mw.Board = Chessboard.StandardChessboard(); }

    public void ClearBoard()
    {
        int row = _mw.Board.Rows;
        int col = _mw.Board.Cols;
        _mw.Board = new Chessboard(row, col);
    }

    public BoardEditorState GetCurrentState() { return EditorExporter.ExportBoardEditorState(_mw); }

}
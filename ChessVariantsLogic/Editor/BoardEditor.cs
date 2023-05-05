using ChessVariantsLogic.Export;

namespace ChessVariantsLogic.Editor;

public class BoardEditor
{
    private MoveWorker _mw;     // This holds the imagepaths for all pieces
    private MoveWorker _mwID;   // This holds the piece ID for the db for all pieces.
    private string? _pieceImage;
    private string? _pieceID;

    public BoardEditor()
    {
        _mw = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        _mwID = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
    }

    public void UpdateBoardSize(int row, int col)
    {
        _mw.Board = new Chessboard(row, col);
        _mwID.Board = new Chessboard(row, col);
    }

    public void InsertPiece(string square)
    {
        if(_pieceImage == null)
            return;
        if(_pieceImage.Equals("remove"))
        {
            _mw.Board.Remove(square);
            _mwID.Board.Remove(square);
        }
        else
        {
            _mw.Board.Insert(_pieceImage, square);
            _mwID.Board.Insert(_pieceImage, square);
        }
    }

    //Parameter "piece" should probably be processed in some way.
    public void SetActivePiece(string pieceID, string pieceImage)
    {
        _pieceID = pieceID;
        _pieceImage = pieceImage;
    }

    public void ResetStartingPosition()
    {
        _mw.Board = Chessboard.StandardChessboard();
        _mwID.Board = Chessboard.StandardChessboard();
    }

    public void ClearBoard()
    {
        int row = _mw.Board.Rows;
        int col = _mw.Board.Cols;
        _mw.Board = new Chessboard(row, col);
        _mwID.Board = new Chessboard(row, col);
    }

    public void SaveBoardToDb()
    {
        // This should use _mwID to save all the piece IDs.
    }

    public BoardEditorState GetCurrentState() { return EditorExporter.ExportBoardEditorState(_mw); }

}
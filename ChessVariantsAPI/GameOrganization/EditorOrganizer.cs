using ChessVariantsLogic.Editor;
using ChessVariantsLogic.Export;

namespace ChessVariantsAPI;

/// <summary>
/// This class is responsible for communicating changes to the editor.
/// </summary>
public class EditorOrganizer
{
    private readonly PieceEditor _pieceEditor;

    public EditorOrganizer()
    {
        this._pieceEditor = new PieceEditor();
    }

    public void ShowMovement(bool enable)
    {
        if(enable)
            _pieceEditor.ShowMovement();
        else
            _pieceEditor.ShowCaptures();
    }

    public void SetBoardSize(int rows, int cols) { _pieceEditor.UpdateBoardSize(rows, cols); }

    public void SetActiveSquare(string square) { _pieceEditor.SetActiveSquare(square); }

    public EditorState GetcurrentState() { return _pieceEditor.GetCurrentState(); }

    //public string GetValidMoves(string square) { return _pieceEditor.GetAllCurrentlyValidMovesFromSquareAsJson(square); }

    //public string GetValidCaptures(string square) { return _pieceEditor.GetAllCurrentlyValidCapturesFromSquareAsJson(square); }

    public void AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this._pieceEditor.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this._pieceEditor.AddCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        return this._pieceEditor.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent RemoveCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        return this._pieceEditor.RemoveCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent BelongsToPlayer(string player) { return _pieceEditor.BelongsToPlayer(player); }

    public void SameMovementAndCapture(bool enable) { _pieceEditor.SetSameMovementAndCapturePattern(enable); }

    public void CanBeCaptured(bool enable) { _pieceEditor.SetCanBeCaptured(enable); }

    public void RepeatMovement(int repeat) { _pieceEditor.RepeatMovement(repeat); }

    public void SetRoyal(bool enable) { _pieceEditor.SetRoyal(enable); }

    public EditorEvent Build() { return _pieceEditor.BuildPiece(); }

    public void Reset() { _pieceEditor.ResetPiece(); }

    public string GetStateAsJson() { return this._pieceEditor.ExportStateAsJson(); }


}
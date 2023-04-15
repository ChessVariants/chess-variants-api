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
        _pieceEditor = new PieceEditor();
    }

    public void ShowMovement(bool enable) { _pieceEditor.ShowMovement(enable); }

    public void SetBoardSize(int rows, int cols) { _pieceEditor.UpdateBoardSize(rows, cols); }

    public void SetActiveSquare(string square) { _pieceEditor.SetActiveSquare(square); }

    public EditorState GetCurrentState() { return _pieceEditor.GetCurrentState(); }

    public PatternState GetCurrentPatternState() { return _pieceEditor.GetCurrentPatternState(); }

    public void AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        _pieceEditor.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        _pieceEditor.AddCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        return _pieceEditor.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
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
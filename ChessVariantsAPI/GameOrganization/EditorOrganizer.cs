using ChessVariantsLogic.Editor;

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

    public string GetValidMoves(string square) { return _pieceEditor.GetAllCurrentlyValidMovesFromSquareAsJson(square); }

    public string GetValidCaptures(string square) { return _pieceEditor.GetAllCurrentlyValidCapturesFromSquareAsJson(square); }

    public void AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this._pieceEditor.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this._pieceEditor.AddCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public void RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this._pieceEditor.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void RemoveCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this._pieceEditor.RemoveCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public void BelongsToPlayer(string player) { _pieceEditor.BelongsToPlayer(player); }

    public void SameMovementAndCapture(bool enable) { _pieceEditor.SetSameMovementAndCapturePattern(enable); }

    public void CanBeCaptured(bool enable) { _pieceEditor.SetCanBeCaptured(enable); }

    public void RepeatMovement(int repeat) { _pieceEditor.RepeatMovement(repeat); }

    public void SetRoyal(bool enable) { _pieceEditor.SetRoyal(enable); }

    public void Build() { _pieceEditor.BuildPiece(); }

    public void Reset() { _pieceEditor.ResetPiece(); }

    public string GetStateAsJson() { return this._pieceEditor.ExportPieceAsJson(); }


}
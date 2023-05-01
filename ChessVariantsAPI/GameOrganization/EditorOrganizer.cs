using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic.Editor;
using ChessVariantsLogic.Export;

namespace ChessVariantsAPI;

/// <summary>
/// This class is responsible for communicating changes to the editor.
/// </summary>
public class EditorOrganizer
{

    private readonly Dictionary<string, PieceEditor?> _activeEditors;

    public EditorOrganizer()
    {
        _activeEditors = new Dictionary<string, PieceEditor?>();
    }

    public PieceEditor CreateEditor(string editorId)
    {
        AssertEditorDoesNotExist(editorId);
        var editor = new PieceEditor();
        _activeEditors.Add(editorId, editor);
        return editor;
    }

    private void AssertEditorDoesNotExist(string editorId)
    {
        var editor = _activeEditors.GetValueOrDefault(editorId, null);
        if (editor != null)
        {
            throw new OrganizerException($"The editor (id: {editorId}) you're trying to create already exists");
        }
    }

    private PieceEditor GetEditor(string editorId)
    {
        var editor = _activeEditors.GetValueOrDefault(editorId, null);
        if (editor == null)
        {
            throw new EditorNotFoundException($"No active editor for editorId: {editorId}");
        }
        return editor;
    }

    public void RemoveAllMovementPatterns(string editorId)
    {
        var editor = GetEditor(editorId);
        editor.RemoveAllMovementPatterns();
    }

    public void ShowMovement(string editorId, bool enable)
    {
        var editor = GetEditor(editorId);
        editor.ShowMovement(enable); 
    }

    public void SetBoardSize(string editorId, int rows, int cols)
    { 
        var editor = GetEditor(editorId);
        editor.UpdateBoardSize(rows, cols);
    }

    public void SetActiveSquare(string editorId, string square)
    {
        var editor = GetEditor(editorId);
        editor.SetActiveSquare(square);
    }

    public EditorState GetCurrentState(string editorId)
    {
        var editor = GetEditor(editorId);
        return editor.GetCurrentState();
    }

    public PatternState GetCurrentPatternState(string editorId)
    {
        var editor = GetEditor(editorId);
        return editor.GetCurrentPatternState();
    }

    public void AddMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        var editor = GetEditor(editorId);
        editor.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void AddCapturePattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        var editor = GetEditor(editorId);
        editor.AddCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent RemoveMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        var editor = GetEditor(editorId);
        return editor.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent BelongsToPlayer(string editorId, string player)
    {
        var editor = GetEditor(editorId);
        return editor.BelongsToPlayer(player);
    }

    public void SameMovementAndCapture(string editorId, bool enable)
    {
        var editor = GetEditor(editorId);
        editor.SetSameMovementAndCapturePattern(enable);
    }

    public void CanBeCaptured(string editorId, bool enable)
    {
        var editor = GetEditor(editorId);
        editor.SetCanBeCaptured(enable);
    }

    public void RepeatMovement(string editorId, int repeat)
    {
        var editor = GetEditor(editorId);
        editor.RepeatMovement(repeat);
    }

    public void SetRoyal(string editorId, bool enable)
    {
        var editor = GetEditor(editorId);
        editor.SetRoyal(enable);
    }

    public EditorEvent Build(string editorId)
    {
        var editor = GetEditor(editorId);
        return editor.BuildPiece();
    }

    public void Reset(string editorId)
    {
        var editor = GetEditor(editorId);
        editor.ResetPiece();
    }

    public string GetStateAsJson(string editorId)
    {
        var editor = GetEditor(editorId);
        return editor.ExportStateAsJson();
    }


}
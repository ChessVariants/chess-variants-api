using ChessVariantsAPI.Controllers;
using ChessVariantsAPI.GameOrganization;
using ChessVariantsAPI.ObjectTranslations;
using ChessVariantsLogic;
using ChessVariantsLogic.Editor;
using ChessVariantsLogic.Export;

namespace ChessVariantsAPI;

/// <summary>
/// This class is responsible for communicating changes to the editor.
/// </summary>
public class EditorOrganizer
{

    private readonly Dictionary<string, PieceEditor?> _activePieceEditors;
    private readonly Dictionary<string, BoardEditor?> _activeBoardEditors;

    public EditorOrganizer()
    {
        _activePieceEditors = new Dictionary<string, PieceEditor?>();
        _activeBoardEditors = new Dictionary<string, BoardEditor?>();
    }

#region Board editor
    public BoardEditor CreateBoardEditor(string editorId)
    {
        AssertPieceEditorDoesNotExist(editorId);
        var editor = new BoardEditor();
        _activeBoardEditors.Add(editorId, editor);
        return editor;
    }

    private void AssertBoardEditorDoesNotExist(string editorId)
    {
        var editor = _activeBoardEditors.GetValueOrDefault(editorId, null);
        if (editor != null)
        {
            throw new OrganizerException($"The editor (id: {editorId}) you're trying to create already exists");
        }
    }

    private BoardEditor GetBoardEditor(string editorId)
    {
        var editor = _activeBoardEditors.GetValueOrDefault(editorId, null);
        if (editor == null)
            throw new EditorNotFoundException($"No active editor for editorId: {editorId}");
        return editor;
    }

    public void SetBoardEditorBoardSize(string editorId, int rows, int cols)
    {
        var editor = GetBoardEditor(editorId);
        editor.UpdateBoardSize(rows, cols);
    }

    public void SetActivePiece(string editorId, Piece piece)
    {
        var editor = GetBoardEditor(editorId);
        editor.SetActivePiece(piece);
    }

    public void SetActiveRemove(string editorId)
    {
        var editor = GetBoardEditor(editorId);
        editor.SetActiveRemove();
    }

    public void InsertPiece(string editorId, string square)
    {
        var editor = GetBoardEditor(editorId);
        editor.UpdateSquare(square);
    }

    public void ResetStartingPosition(string editorId)
    {
        var editor = GetBoardEditor(editorId);
        editor.ResetStartingPosition();
    }

    public void ClearBoard(string editorId)
    {
        var editor = GetBoardEditor(editorId);
        editor.ClearBoard();
    }

    public BoardEditorState GetCurrentBoardEditorState(string editorId)
    {
        var editor = GetBoardEditor(editorId);
        return editor.GetCurrentState();
    }

#endregion

#region Piece editor
    public PieceEditor CreatePieceEditor(string editorId)
    {
        AssertPieceEditorDoesNotExist(editorId);
        var editor = new PieceEditor();
        _activePieceEditors.Add(editorId, editor);
        return editor;
    }

    private void AssertPieceEditorDoesNotExist(string editorId)
    {
        var editor = _activePieceEditors.GetValueOrDefault(editorId, null);
        if (editor != null)
            throw new OrganizerException($"The editor (id: {editorId}) you're trying to create already exists");
    }

    private PieceEditor GetPieceEditor(string editorId)
    {
        var editor = _activePieceEditors.GetValueOrDefault(editorId, null);
        if (editor == null)
            throw new EditorNotFoundException($"No active editor for editorId: {editorId}");
        return editor;
    }

    public void SetImagePath(string editorId, string imagePath)
    {
        var editor = GetPieceEditor(editorId);
        editor.SetImagePath(imagePath);
    }

    public void RemoveAllMovementPatterns(string editorId)
    {
        var editor = GetPieceEditor(editorId);
        editor.RemoveAllMovementPatterns();
    }

    public void ShowMovement(string editorId, bool enable)
    {
        var editor = GetPieceEditor(editorId);
        editor.ShowMovement(enable); 
    }

    public void SetPieceEditorBoardSize(string editorId, int rows, int cols)
    { 
        var editor = GetPieceEditor(editorId);
        editor.UpdateBoardSize(rows, cols);
    }

    public void SetActiveSquare(string editorId, string square)
    {
        var editor = GetPieceEditor(editorId);
        editor.SetActiveSquare(square);
    }

    public PieceEditorState GetCurrentPieceEditorState(string editorId)
    {
        var editor = GetPieceEditor(editorId);
        return editor.GetCurrentState();
    }

    public PatternState GetCurrentPatternState(string editorId)
    {
        var editor = GetPieceEditor(editorId);
        return editor.GetCurrentPatternState();
    }

    public void AddMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        var editor = GetPieceEditor(editorId);
        editor.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void AddCapturePattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        var editor = GetPieceEditor(editorId);
        editor.AddCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent RemoveMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        var editor = GetPieceEditor(editorId);
        return editor.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent BelongsToPlayer(string editorId, string player)
    {
        var editor = GetPieceEditor(editorId);
        return editor.BelongsToPlayer(player);
    }

    public void SameMovementAndCapture(string editorId, bool enable)
    {
        var editor = GetPieceEditor(editorId);
        editor.SetSameMovementAndCapturePattern(enable);
    }

    public void CanBeCaptured(string editorId, bool enable)
    {
        var editor = GetPieceEditor(editorId);
        editor.SetCanBeCaptured(enable);
    }

    public void RepeatMovement(string editorId, int repeat)
    {
        var editor = GetPieceEditor(editorId);
        editor.RepeatMovement(repeat);
    }

    public void SetRoyal(string editorId, bool enable)
    {
        var editor = GetPieceEditor(editorId);
        editor.SetRoyal(enable);
    }

    public Piece? Build(string editorId)
    {
        var editor = GetPieceEditor(editorId);
        return editor.BuildPiece();
    }

    public void Reset(string editorId)
    {
        var editor = GetPieceEditor(editorId);
        editor.ResetPiece();
    }


#endregion

}
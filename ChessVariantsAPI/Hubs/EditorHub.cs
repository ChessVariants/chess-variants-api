using Microsoft.AspNetCore.SignalR;
using ChessVariantsLogic.Export;
using ChessVariantsLogic.Editor;

namespace ChessVariantsAPI.Hubs;

public class EditorHub : Hub
{

    private readonly EditorOrganizer _organizer;
    readonly protected ILogger _logger;

    public EditorHub(EditorOrganizer organizer, ILogger<EditorHub> logger)
    {
        _organizer = organizer;
        _logger = logger;
    }

    #region BoardEditor

    public BoardEditorState RequestBoardEditorState(string editorId) { return _organizer.GetCurrentBoardEditorState(editorId); }
    public async Task CreateBoardEditor(string editorId)
    {
        _organizer.CreateBoardEditor(editorId);
        await UpdateBoardEditorState(editorId);
    }

    private async Task UpdateBoardEditorState(string editorId)
    {
        var state = _organizer.GetCurrentBoardEditorState(editorId);
        await Clients.Caller.SendUpdatedBoardEditorState(state);
    }

    public async Task SetBoardEditorSize(string editorId, int row, int col)
    {
        _organizer.SetBoardEditorBoardSize(editorId, row, col);
        await UpdateBoardEditorState(editorId);
    }

    public async Task SetActivePiece(string editorId, string piece)
    {
        _organizer.SetActivePiece(editorId, piece);
        await UpdateBoardEditorState(editorId);
    }

    public async Task UpdateSquare(string editorId, string square)
    {
        _organizer.InsertPiece(editorId, square);
        await UpdateBoardEditorState(editorId);
    }

    public async Task ResetStartingPosition(string editorId)
    {
        _organizer.ResetStartingPosition(editorId);
        await UpdateBoardEditorState(editorId);
    }

    public async Task ClearBoard(string editorId)
    {
        _organizer.ClearBoard(editorId);
        await UpdateBoardEditorState(editorId);
    }

    #endregion

    #region PieceEditor
    public async Task CreatePieceEditor(string editorId)
    {
        _organizer.CreatePieceEditor(editorId);
        await UpdatePieceEditorState(editorId);
        await UpdatePatternState(editorId);
    }

    public PieceEditorState RequestPieceEditorState(string editorId) { return _organizer.GetCurrentPieceEditorState(editorId); }
    public PatternState RequestPatternState(string editorId) { return _organizer.GetCurrentPatternState(editorId); }

    private async Task UpdatePatternState(string editorId)
    {
        var patternState = _organizer.GetCurrentPatternState(editorId);
        await Clients.Caller.SendUpdatedPatternState(patternState);
    }

    private async Task UpdatePieceEditorState(string editorId)
    {
        var state = _organizer.GetCurrentPieceEditorState(editorId);
        await Clients.Caller.SendUpdatedPieceEditorState(state);
    }

    public async Task ActivateSquare(string editorId, string square)
    {
        _organizer.SetActiveSquare(editorId, square);
        await UpdatePieceEditorState(editorId);
    }

    public async Task AddMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddMovementPattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task RemoveMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.RemoveMovementPattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task AddCapturePattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddCapturePattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task UpdatePieceEditorBoardSize(string editorId, int rows, int cols)
    {
        _organizer.SetPieceEditorBoardSize(editorId, rows, cols);
        await UpdatePieceEditorState(editorId);
    }

    public async Task SetCaptureSameAsMovement(string editorId, bool enable)
    {
        _organizer.SameMovementAndCapture(editorId, enable);
        await UpdatePieceEditorState(editorId);
    }

    public async Task ShowMovement(string editorId, bool enable)
    {
        _organizer.ShowMovement(editorId, enable);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task PieceCanBeCaptured(string editorId, bool enable)
    {
        _organizer.CanBeCaptured(editorId, enable);
        await UpdatePieceEditorState(editorId);
    }

    public async Task BelongsToPlayer(string editorId, string player)
    {
        _organizer.BelongsToPlayer(editorId, player);
        await UpdatePieceEditorState(editorId);
    }

    public async Task AmountRepeat(string editorId, int repeat)
    {
        _organizer.RepeatMovement(editorId, repeat);
        await UpdatePieceEditorState(editorId);
    }

    public async Task ClearMovementPatterns(string editorId)
    {
        _organizer.RemoveAllMovementPatterns(editorId);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task ResetPiece(string editorId)
    {
        _organizer.Reset(editorId);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task BuildPiece(string editorId)
    {
        var buildEvent = _organizer.Build(editorId);
        if (buildEvent.Equals(EditorEvent.BuildFailed))
            await Clients.Caller.SendBuildFailed();
        else
        {
            await UpdatePatternState(editorId);
            await UpdatePieceEditorState(editorId);

        }
    }

    #endregion

}
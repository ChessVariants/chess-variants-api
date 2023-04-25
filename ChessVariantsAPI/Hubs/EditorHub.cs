using Microsoft.AspNetCore.SignalR;
using ChessVariantsLogic.Export;
using ChessVariantsAPI.Hubs.DTOs;
using static ChessVariantsAPI.Hubs.GameHub;
using ChessVariantsAPI.GameOrganization;

namespace ChessVariantsAPI.Hubs;

public class EditorHub : Hub
{

    private readonly EditorOrganizer _organizer;
    private readonly GroupOrganizer _groupOrganizer;
    readonly protected ILogger _logger;

    public EditorHub(EditorOrganizer organizer, ILogger<EditorHub> logger, GroupOrganizer groupOrganizer)
    {
        _organizer = organizer;
        _logger = logger;
        _groupOrganizer = groupOrganizer;
    }

    public async Task CreateEditor(string editorId)
    { 
        _organizer.CreateEditor(editorId);
        await UpdateEditorState(editorId);
        await UpdatePatternState(editorId);
    }

    private string GetUsername()
    {
        var username = Context.GetUsername(); // returns null for some reason.
        if (username == null)
        {
            _logger.LogInformation("Unauthenticated request by connection: {connId}", Context.ConnectionId);
            throw new AuthenticationError(Events.Errors.UnauthenticatedRequest);
        }
        return username;
    }

    public EditorState RequestState(string editorId) { return _organizer.GetCurrentState(editorId); }
    public PatternState RequestPatternState(string editorId) { return _organizer.GetCurrentPatternState(editorId); }

    public async Task UpdatePatternState(string editorId)
    {
        var patternState = _organizer.GetCurrentPatternState(editorId);
        await Clients.Caller.SendUpdatedPatternState(patternState);
    }

    public async Task UpdateEditorState(string editorId)
    {
        var state = _organizer.GetCurrentState(editorId);
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task ActivateSquare(string editorId, string square)
    {
        _organizer.SetActiveSquare(editorId, square);
        await UpdateEditorState(editorId);
    }

    public async Task AddMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddMovementPattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdateEditorState(editorId);
    }

    public async Task RemoveMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.RemoveMovementPattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdateEditorState(editorId);
    }

    public async Task AddCapturePattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddCapturePattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdateEditorState(editorId);
    }

    public async Task UpdateBoardSize(string editorId, int rows, int cols)
    {
        _organizer.SetBoardSize(editorId, rows, cols);
        await UpdateEditorState(editorId);
    }

    public async Task SetCaptureSameAsMovement(string editorId, bool enable)
    {
        _organizer.SameMovementAndCapture(editorId, enable);
        await UpdateEditorState(editorId);
    }
    
    public async Task ShowMovement(string editorId, bool enable)
    {
        _organizer.ShowMovement(editorId, enable);
        await UpdatePatternState(editorId);
        await UpdateEditorState(editorId);
    }

    public async Task PieceCanBeCaptured(string editorId, bool enable)
    {
        _organizer.CanBeCaptured(editorId, enable);
        await UpdateEditorState(editorId);
    }

    public async Task BelongsToPlayer(string editorId, string player)
    {
        _organizer.BelongsToPlayer(editorId, player);
        await UpdateEditorState(editorId);
    }

    public async Task AmountRepeat(string editorId, int repeat)
    {
        _organizer.RepeatMovement(editorId, repeat);
        await UpdateEditorState(editorId);
    }

    public async Task ClearMovementPatterns(string editorId)
    {
        _organizer.RemoveAllMovementPatterns(editorId);
        await UpdatePatternState(editorId);
        await UpdateEditorState(editorId);
    }

    public async Task ResetPiece(string editorId)
    {
        _organizer.Reset(editorId);
        await UpdatePatternState(editorId);
        await UpdateEditorState(editorId);
    }

}
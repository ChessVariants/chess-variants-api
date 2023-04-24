using Microsoft.AspNetCore.SignalR;
using ChessVariantsLogic.Export;

namespace ChessVariantsAPI.Hubs;

public class EditorHub : Hub
{

    private readonly EditorOrganizer _organizer;

    public EditorHub(EditorOrganizer organizer)
    {
        this._organizer = organizer;
    }

    public EditorState RequestState() { return _organizer.GetCurrentState(); }

    public PatternState RequestPatternState() { return _organizer.GetCurrentPatternState(); }

    public async Task UpdatePatternState()
    {
        var patternState = _organizer.GetCurrentPatternState();
        await Clients.Caller.SendUpdatedPatternState(patternState);
    }

    public async Task UpdateEditorState()
    {
        var state = _organizer.GetCurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task ActivateSquare(string square)
    {
        _organizer.SetActiveSquare(square);
        await UpdateEditorState();
    }

    public async Task AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddMovementPattern(xDir, yDir, minLength, maxLength);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddCapturePattern(xDir, yDir, minLength, maxLength);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task UpdateBoardSize(int rows, int cols)
    {
        _organizer.SetBoardSize(rows, cols);
        await UpdateEditorState();
    }

    public async Task SetCaptureSameAsMovement(bool enable)
    {
        _organizer.SameMovementAndCapture(enable);
        await UpdateEditorState();
    }
    
    public async Task ShowMovement(bool enable)
    {
        _organizer.ShowMovement(enable);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task PieceCanBeCaptured(bool enable)
    {
        _organizer.CanBeCaptured(enable);
        await UpdateEditorState();
    }

    public async Task BelongsToPlayer(string player)
    {
        _organizer.BelongsToPlayer(player);
        await UpdateEditorState();
    }

    public async Task AmountRepeat(int repeat)
    {
        _organizer.RepeatMovement(repeat);
        await UpdateEditorState();
    }

    public async Task ClearMovementPatterns()
    {
        _organizer.RemoveAllMovementPatterns();
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task ResetPiece()
    {
        _organizer.Reset();
        await UpdatePatternState();
        await UpdateEditorState();
    }

}
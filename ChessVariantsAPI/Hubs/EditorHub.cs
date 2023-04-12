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

    public EditorState RequestState()
    {
        Console.WriteLine("In RequestState editorHub");
        return _organizer.GetCurrentState();
    }

    public PatternState RequestPatternState()
    {
        Console.WriteLine("In RequestPatternState editorHub");
        return _organizer.GetCurrentPatternState();
    }

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
        Console.WriteLine("Activating square");
        _organizer.SetActiveSquare(square);
        await UpdateEditorState();
    }

    public async Task AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        Console.WriteLine("Adding movementPattern");
        _organizer.AddMovementPattern(xDir, yDir, minLength, maxLength);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        Console.WriteLine("Removing movementPattern");
        _organizer.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        Console.WriteLine("Adding CapturePattern");
        _organizer.AddCapturePattern(xDir, yDir, minLength, maxLength);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task UpdateBoardSize(int rows, int cols)
    {
        Console.WriteLine("Setting board size");
        _organizer.SetBoardSize(rows, cols);
        await UpdateEditorState();
    }

    public async Task SetCaptureSameAsMovement(bool enable)
    {
        Console.WriteLine("Setting capture and movement");
        _organizer.SameMovementAndCapture(enable);
        await UpdateEditorState();
    }
    
    public async Task ShowMovement(bool enable)
    {
        Console.WriteLine("Show movement or capture");
        _organizer.ShowMovement(enable);
        await UpdatePatternState();
        await UpdateEditorState();
    }

    public async Task PieceCanBeCaptured(bool enable)
    {
        Console.WriteLine("Set CanBeCaptured");
        _organizer.CanBeCaptured(enable);
        await UpdateEditorState();
    }

    public async Task BelongsToPlayer(string player)
    {
        Console.WriteLine("Set belongs to player");
        _organizer.BelongsToPlayer(player);
        await UpdateEditorState();
    }

    public async Task AmountRepeat(int repeat)
    {
        Console.WriteLine("Set repeat");
        _organizer.RepeatMovement(repeat);
        await UpdateEditorState();
    }


}
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
        return _organizer.GetcurrentState();
    }

    public async Task ActivateSquare(string square)
    {
        Console.WriteLine("Activating square");
        _organizer.SetActiveSquare(square);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        Console.WriteLine("Adding movementPattern");
        _organizer.AddMovementPattern(xDir, yDir, minLength, maxLength);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        Console.WriteLine("Removing movementPattern");
        _organizer.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        Console.WriteLine("Adding CapturePattern");
        _organizer.AddCapturePattern(xDir, yDir, minLength, maxLength);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task RemoveCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        Console.WriteLine("Removing capturePattern");
        _organizer.RemoveCapturePattern(xDir, yDir, minLength, maxLength);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task UpdateBoardSize(int rows, int cols)
    {
        Console.WriteLine("Setting board size");
        _organizer.SetBoardSize(rows, cols);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }

    public async Task SetCaptureSameAsMovement(bool enable)
    {
        Console.WriteLine("Setting capture and movement");
        _organizer.SameMovementAndCapture(enable);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }
    
    public async Task ShowMovement(bool enable)
    {
        Console.WriteLine("Show movement or capture");
        _organizer.ShowMovement(enable);
        var state = _organizer.GetcurrentState();
        await Clients.Caller.SendUpdatedEditorState(state);
    }


}
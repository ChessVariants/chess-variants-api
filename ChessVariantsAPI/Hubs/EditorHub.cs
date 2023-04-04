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
        var state = _organizer.GetcurrentState();
        Console.WriteLine("State: " + state.Moves.ToString());
        return state;
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

}
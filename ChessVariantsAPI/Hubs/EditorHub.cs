using ChessVariantsLogic;
using Microsoft.AspNetCore.SignalR;

namespace ChessVariantsAPI.Hubs;

public class EditorHub : Hub
{

    private readonly EditorOrganizer _organizer;

    public EditorHub(EditorOrganizer organizer)
    {
        this._organizer = organizer;
    }

    public async Task AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        EditorEvent? result = null;
        string? state = null;
        try
        {
            result = this._organizer.AddMovementPattern(xDir, yDir, minLength, maxLength);
            state = _organizer.GetStateAsJson();
        }
        catch(Exception e)
        {
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }

        switch (result)
        {
            case EditorEvent.Success:
                await Clients.Caller.SendAsync(Events.Success, state);
                return;
            case EditorEvent.InvalidMovementPattern:
                await Clients.Caller.SendAsync(Events.InvalidMovementPattern, "Invalid movement pattern");
                return;
        }
    }

    private static class Events
    {
        public readonly static string Success = "success";
        public readonly static string InvalidMovementPattern = "invalidMovementPattern";
        public readonly static string Error = "error";

    }

}
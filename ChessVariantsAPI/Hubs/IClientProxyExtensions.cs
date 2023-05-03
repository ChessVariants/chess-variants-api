using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic.Export;
using Microsoft.AspNetCore.SignalR;

namespace ChessVariantsAPI.Hubs;

/// <summary>
/// This class contains extensions for <see cref="IClientProxy"/> in order to simplify syntax in
/// <see cref="GameHub"/> and also enforce that events of the same type always return the same
/// type of data. i.e an UpdatedGameState event should always include the new state.
/// </summary>
public static class IClientProxyExtensions
{
    public static async Task SendGameJoined(this IClientProxy clients, string playerColor, string username)
    {
        await clients.SendAsync(Events.GameJoined, playerColor, username);
    }

    public static async Task SendGameCreated(this IClientProxy clients, string playerColor, string username)
    {
        await clients.SendAsync(Events.GameCreated, playerColor, username);
    }

    public static async Task SendPlayerJoinedGame(this IClientProxy clients, string playerColor, string username)
    {
        await clients.SendAsync(Events.PlayerJoinedGame, playerColor, username);
    }

    public static async Task SendGenericError(this IClientProxy clients, string errorMessage)
    {
        await clients.SendAsync(Events.Error, errorMessage);
    }

    public static async Task SendGameVariantSet(this IClientProxy clients, string variantIdentifier)
    {
        await clients.SendAsync(Events.GameVariantSet, variantIdentifier);
    }

    public static async Task SendGameNotVariantSet(this IClientProxy clients, string message)
    {
        await clients.SendAsync(Events.GameVariantNotSet, message);
    }

    public static async Task SendGameStarted(this IClientProxy clients, ColorsDTO colors)
    {
        await clients.SendAsync(Events.GameStarted, colors);
    }

    public static async Task SendUpdatedGameState(this IClientProxy clients, GameState state)
    {
        await clients.SendAsync(Events.UpdatedGameState, state);
    }

    public static async Task SendGameLeft(this IClientProxy clients)
    {
        await clients.SendAsync(Events.GameLeft);
    }

    public static async Task SendPlayerLeftGame(this IClientProxy clients, string username)
    {
        await clients.SendAsync(Events.PlayerLeftGame, username);
    }

    public static async Task SendColors(this IClientProxy clients, ColorsDTO colors)
    {
        await clients.SendAsync(Events.Colors, colors);
    }

    public static async Task SendBlackWon(this IClientProxy clients)
    {
        await clients.SendAsync(Events.BlackWon);
    }

    public static async Task SendWhiteWon(this IClientProxy clients)
    {
        await clients.SendAsync(Events.WhiteWon);
    }

    public static async Task SendTie(this IClientProxy clients)
    {
        await clients.SendAsync(Events.Tie);
    }

    public static async Task SendInvalidMove(this IClientProxy clients)
    {
        await clients.SendAsync(Events.InvalidMove);
    }

    public static async Task SendUpdatedBoardEditorState(this IClientProxy clients, BoardEditorState state)
    {
        await clients.SendAsync(Events.UpdatedBoardEditorState, state);
    }

    public static async Task SendUpdatedPieceEditorState(this IClientProxy clients, PieceEditorState state)
    {
        await clients.SendAsync(Events.UpdatedPieceEditorState, state);
    }

    public static async Task SendUpdatedPatternState(this IClientProxy clients, PatternState state)
    {
        await clients.SendAsync(Events.PatternAdded, state);
    }

    public static async Task SendBuildFailed(this IClientProxy clients)
    {
        await clients.SendAsync(Events.BuildFailed);
    }
}

public static class Events
{
    public readonly static string GameNotFound = "gameNotFound";
    public readonly static string PieceMoved = "pieceMoved";
    public readonly static string UpdatedGameState = "updatedGameState";
    public readonly static string PlayerLeftGame = "playerLeftGame";
    public readonly static string PlayerJoinedGame = "playerJoinedGame";
    public readonly static string GameCreated = "gameCreated";
    public readonly static string GameJoined = "gameJoined";
    public readonly static string GameLeft = "gameLeft";
    public readonly static string PlayerNotFound = "playerNotFound";
    public readonly static string InvalidMove = "invalidMove";
    public readonly static string Error = "error";
    public readonly static string WhiteWon = "whiteWon";
    public readonly static string BlackWon = "blackWon";
    public readonly static string Tie = "tie";
    public readonly static string Colors = "colors";

#region Lobby events
    public readonly static string PlayerJoinedLobby = "playerJoinedLobby";
    public readonly static string PlayerLeftLobby = "playerLeftLobby";
    public readonly static string LobbyCreated = "lobbyCreated";
    public readonly static string LobbyJoined = "lobbyJoined";
    public readonly static string GameVariantSet = "gameVariantSet";
    public readonly static string GameVariantNotSet = "gameVariantNotSet";
    public readonly static string GameStarted = "gameStarted";

#endregion

#region Editor events

    public readonly static string UpdatedPieceEditorState = "updatedPieceEditorState";
    public readonly static string UpdatedBoardEditorState = "updatedBoardEditorState";
    public readonly static string PatternAdded = "PatternAdded";
    public readonly static string BuildFailed = "buildFailed";

#endregion

    public static class Errors
    {
        public readonly static string UnauthenticatedRequest = "unauthenticatedRequest";
    }
}
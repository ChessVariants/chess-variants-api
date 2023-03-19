using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChessVariantsAPI.Hubs;

/// <summary>
/// Convenience methods for getting potential claim information from <see cref="HubCallerContext"/>
/// </summary>
public static class ContextCallerExtensions
{
    public static string? GetEmail(this HubCallerContext context)
    {
        return context.User?.FindFirst(ClaimTypes.Email)?.Value!;
    }

    public static string? GetUsername(this HubCallerContext context)
    {
        return context.User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}

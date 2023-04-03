using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChessVariantsAPI.Authentication;

/// <summary>
/// This class is used by SignalR to sign the UserIdentity claims.
/// </summary>
public class UsernameBasedUserIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}

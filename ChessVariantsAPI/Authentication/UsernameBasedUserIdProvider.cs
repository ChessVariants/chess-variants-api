using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChessVariantsAPI.Authentication;

public class UsernameBasedUserIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}

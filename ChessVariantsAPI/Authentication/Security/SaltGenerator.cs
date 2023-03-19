using System.Security.Cryptography;

namespace ChessVariantsAPI.Authentication;

public static class SaltGenerator
{
    private readonly static int saltLength = 32;

    public static string GetSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(saltLength));
    }
}

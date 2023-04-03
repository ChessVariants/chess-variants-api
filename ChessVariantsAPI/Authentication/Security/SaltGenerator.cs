using System.Security.Cryptography;

namespace ChessVariantsAPI.Authentication;

/// <summary>
/// This class has functionality for generating a random salt with 32 characters.
/// </summary>
public static class SaltGenerator
{
    private readonly static int saltLength = 32;

    public static string GetSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(saltLength));
    }
}

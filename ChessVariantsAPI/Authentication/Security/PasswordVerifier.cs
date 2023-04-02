namespace ChessVariantsAPI.Authentication;

/// <summary>
/// This class verifies that a password is correct for any implementation of <see cref="PasswordHasher"/>
/// </summary>
public static class PasswordVerifier
{
    public static bool Verify(string unhashedPassword, string storedHashedPassword, string salt, PasswordHasher hasher)
    {
        var hashedPassword = hasher.Hash(unhashedPassword, salt);
        if (hashedPassword != storedHashedPassword)
        {
            return false;
        }
        return true;
    }
}

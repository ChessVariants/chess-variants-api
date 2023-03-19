namespace ChessVariantsAPI.Authentication;

/// <summary>
/// Abstract definition of a password hasher.
/// </summary>
public abstract class PasswordHasher
{
    public string GenerateSalt()
    {
        return SaltGenerator.GetSalt();
    }

    public abstract string Algorithm();
    public abstract string Hash(string password, string salt = "");

}

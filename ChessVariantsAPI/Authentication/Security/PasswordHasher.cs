namespace ChessVariantsAPI.Authentication;

public abstract class PasswordHasher
{
    public string GenerateSalt()
    {
        return SaltGenerator.GetSalt();
    }

    public abstract string Algorithm();
    public abstract string Hash(string password, string salt = "");

}

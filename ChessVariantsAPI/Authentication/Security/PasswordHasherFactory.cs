namespace ChessVariantsAPI.Authentication;

/// <summary>
/// Factory for hashing algorithms
/// </summary>
public static class PasswordHasherFactory
{
    public static PasswordHasher SHA256()
    {
        return new SHA256Hasher();
    }
}
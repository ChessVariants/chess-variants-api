namespace ChessVariantsAPI.Authentication;

public static class PasswordHasherFactory
{
    public static PasswordHasher SHA256()
    {
        return new SHA256Hasher();
    }
}
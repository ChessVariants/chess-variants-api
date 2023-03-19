using System.Security.Cryptography;
using System.Text;

namespace ChessVariantsAPI.Authentication;

public class SHA256Hasher : PasswordHasher
{
    public override string Algorithm()
    {
        return "SHA256";
    }

    public override string Hash(string password, string salt = "")
    {
        StringBuilder sb = new();
        using var sha256 = SHA256.Create();
        var saltedPassword = password + salt;
        byte[] saltedPasswordAsBytes = Encoding.UTF8.GetBytes(saltedPassword);
        byte[] hash = sha256.ComputeHash(saltedPasswordAsBytes);

        foreach (byte b in hash)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}

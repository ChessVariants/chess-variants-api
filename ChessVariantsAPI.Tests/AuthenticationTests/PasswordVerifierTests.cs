using ChessVariantsAPI.Authentication;
using Xunit;

namespace ChessVariantsAPI.Tests.AuthenticationTests;
public class PasswordVerifierTests
{
    [Fact]
    public void Verify_ShouldBeTrue()
    {
        var expected = "58ee99ab3b809689998962df3699789b6b9bbde660809cb1571199e9376b4264"; // SHA256 hash of abc123salt
        var hasher = PasswordHasherFactory.SHA256();
        Assert.True(PasswordVerifier.Verify("abc123", expected, "salt", hasher));
    }

    [Fact]
    public void Verify_ShouldBeFalse()
    {
        var expected = "58ee99ab3b809689998962df3699789b6b9bbde660809cb1571199e9376b4264"; // SHA256 hash of abc123salt
        var hasher = PasswordHasherFactory.SHA256();
        Assert.False(PasswordVerifier.Verify("abc1234", expected, "salt", hasher));
    }
}

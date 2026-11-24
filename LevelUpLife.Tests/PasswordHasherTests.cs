using LevelUpLife.Core.Security;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Модульные тесты хеширования паролей.
/// </summary>
public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_DoesNotContainPassword()
    {
        string hash = _hasher.Hash("secret1");

        Assert.DoesNotContain("secret1", hash);
    }

    [Fact]
    public void Hash_SamePasswordGivesDifferentHashes()
    {
        Assert.NotEqual(_hasher.Hash("secret1"), _hasher.Hash("secret1"));
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        string hash = _hasher.Hash("secret1");

        Assert.True(_hasher.Verify("secret1", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        string hash = _hasher.Hash("secret1");

        Assert.False(_hasher.Verify("secret2", hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("не хеш")]
    [InlineData("1.2")]
    [InlineData("abc.def.ghi")]
    public void Verify_BrokenHash_ReturnsFalse(string brokenHash)
    {
        Assert.False(_hasher.Verify("secret1", brokenHash));
    }
}

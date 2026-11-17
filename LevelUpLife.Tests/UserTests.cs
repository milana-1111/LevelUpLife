using LevelUpLife.Core.Models;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Модульные тесты пользователя и сессии
/// </summary>
public class UserTests
{
    [Fact]
    public void AddXp_IncreasesTotalXp()
    {
        var user = new User { TotalXp = 40 };

        user.AddXp(20);

        Assert.Equal(60, user.TotalXp);
    }

    [Fact]
    public void AddXp_NegativeValue_Throws()
    {
        var user = new User();

        Assert.Throws<ArgumentOutOfRangeException>(() => user.AddXp(-1));
    }

    [Fact]
    public void Session_IsExpired_WhenExpiresAtInPast()
    {
        var session = new Session { ExpiresAt = DateTime.UtcNow.AddMinutes(-1) };

        Assert.True(session.IsExpired());
    }

    [Fact]
    public void Session_IsNotExpired_WhenExpiresAtInFuture()
    {
        var session = new Session { ExpiresAt = DateTime.UtcNow.AddDays(1) };

        Assert.False(session.IsExpired());
    }
}

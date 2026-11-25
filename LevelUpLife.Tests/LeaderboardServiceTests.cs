using LevelUpLife.Core.Models;
using LevelUpLife.Core.Services;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Тесты таблицы лидеров.
/// </summary>
public class LeaderboardServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly LeaderboardService _service;

    /// <summary>
    /// Создаёт сервис и наполняет базу игроками.
    /// </summary>
    public LeaderboardServiceTests()
    {
        _service = new LeaderboardService(_database);

        using var db = _database.CreateDbContext();
        db.Users.AddRange(
            new User { Username = "a", PasswordHash = "h", DisplayName = "Анна", TotalXp = 300, Level = 3 },
            new User { Username = "b", PasswordHash = "h", DisplayName = "Борис", TotalXp = 100, Level = 2 },
            new User { Username = "c", PasswordHash = "h", DisplayName = "Вера", TotalXp = 100, Level = 2 },
            new User { Username = "d", PasswordHash = "h", DisplayName = "Глеб", TotalXp = 50, Level = 1 });
        db.SaveChanges();
    }

    /// <summary>
    /// Освобождает базу данных.
    /// </summary>
    public void Dispose()
    {
        _database.Dispose();
    }

    [Fact]
    public void GetTop_SortsByXpDescending()
    {
        var top = _service.GetTop(10);

        Assert.Equal(new[] { "Анна", "Борис", "Вера", "Глеб" }, top.Select(e => e.DisplayName).ToArray());
    }

    [Fact]
    public void GetTop_EqualXpGivesEqualRank()
    {
        var ranks = _service.GetTop(10).Select(e => e.Rank).ToArray();

        Assert.Equal(new[] { 1, 2, 2, 4 }, ranks);
    }

    [Fact]
    public void GetTop_LimitsCount()
    {
        Assert.Equal(2, _service.GetTop(2).Count);
    }

    [Fact]
    public void GetTop_ZeroCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.GetTop(0));
    }

    [Fact]
    public void GetRank_ReturnsPlaceOfUser()
    {
        using var db = _database.CreateDbContext();
        int firstId = db.Users.Single(u => u.Username == "a").Id;
        int lastId = db.Users.Single(u => u.Username == "d").Id;
        int tiedId = db.Users.Single(u => u.Username == "c").Id;

        Assert.Equal(1, _service.GetRank(firstId));
        Assert.Equal(4, _service.GetRank(lastId));
        Assert.Equal(2, _service.GetRank(tiedId));
    }

    [Fact]
    public void GetRank_UnknownUser_ReturnsZero()
    {
        Assert.Equal(0, _service.GetRank(9999));
    }
}

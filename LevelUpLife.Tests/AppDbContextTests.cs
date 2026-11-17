using LevelUpLife.Core.Data;
using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LevelUpLife.Tests;

public class AppDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;

    /// <summary>
    /// Создаёт чистую базу в памяти перед каждым тестом
    /// </summary>
    public AppDbContextTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    /// <summary>
    /// Освобождает контекст и соединение
    /// </summary>
    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private static User CreateUser(string username)
    {
        return new User { Username = username, PasswordHash = "hash", DisplayName = username };
    }

    [Fact]
    public void Categories_AreSeeded()
    {
        Assert.Equal(5, _db.Categories.Count());
    }

    [Fact]
    public void Username_MustBeUnique()
    {
        _db.Users.Add(CreateUser("milana"));
        _db.SaveChanges();

        _db.Users.Add(CreateUser("milana"));

        Assert.Throws<DbUpdateException>(() => _db.SaveChanges());
    }

    [Fact]
    public void HabitLog_SameHabitAndDate_Throws()
    {
        var user = CreateUser("milana");
        var habit = new Habit { User = user, CategoryId = 1, Title = "Зарядка", Difficulty = Difficulty.Easy };
        habit.Logs.Add(new HabitLog { LogDate = new DateTime(2026, 11, 16) });
        _db.Habits.Add(habit);
        _db.SaveChanges();

        _db.HabitLogs.Add(new HabitLog { HabitId = habit.Id, LogDate = new DateTime(2026, 11, 16) });

        Assert.Throws<DbUpdateException>(() => _db.SaveChanges());
    }

    [Fact]
    public void DeleteUser_RemovesHabitsAndLogs()
    {
        var user = CreateUser("milana");
        var habit = new Habit { User = user, CategoryId = 1, Title = "Зарядка", Difficulty = Difficulty.Easy };
        habit.Logs.Add(new HabitLog { LogDate = new DateTime(2026, 11, 16) });
        _db.Habits.Add(habit);
        _db.SaveChanges();

        _db.Users.Remove(user);
        _db.SaveChanges();

        Assert.Empty(_db.Habits);
        Assert.Empty(_db.HabitLogs);
    }

    [Fact]
    public void Habit_DifficultyAndStatus_AreSavedAndLoaded()
    {
        var user = CreateUser("milana");
        var habit = new Habit { User = user, CategoryId = 2, Title = "Учить C#", Difficulty = Difficulty.Hard };
        var log = new HabitLog { LogDate = new DateTime(2026, 11, 16) };
        log.Complete(30);
        habit.Logs.Add(log);
        _db.Habits.Add(habit);
        _db.SaveChanges();
        _db.ChangeTracker.Clear();

        var loaded = _db.Habits.Include(h => h.Logs).Single();

        Assert.Equal(Difficulty.Hard, loaded.Difficulty);
        Assert.Equal(HabitStatus.Completed, loaded.Logs[0].Status);
        Assert.Equal(30, loaded.Logs[0].XpEarned);
    }
}

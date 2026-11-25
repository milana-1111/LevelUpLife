using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Services;
using LevelUpLife.Core.Validation;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Тесты фасада работы с привычками: создание, отметка, опыт и уровни.
/// </summary>
public class HabitServiceTests : IDisposable
{
    // 16 ноября 2026 — понедельник, 17 — вторник.
    private static readonly DateTime Monday = new(2026, 11, 16);
    private static readonly DateTime Tuesday = new(2026, 11, 17);

    private readonly TestDatabase _database = new();
    private readonly HabitService _service;
    private readonly int _userId;

    /// <summary>
    /// Создаёт сервис и тестового пользователя.
    /// </summary>
    public HabitServiceTests()
    {
        _service = new HabitService(_database, new XpCalculator(), new InputValidator());
        _userId = CreateUser("milana");
    }

    /// <summary>
    /// Освобождает базу данных.
    /// </summary>
    public void Dispose()
    {
        _database.Dispose();
    }

    private int CreateUser(string username, int totalXp = 0)
    {
        using var db = _database.CreateDbContext();
        var user = new User { Username = username, PasswordHash = "hash", DisplayName = username, TotalXp = totalXp };
        db.Users.Add(user);
        db.SaveChanges();
        return user.Id;
    }

    private User GetUser(int id)
    {
        using var db = _database.CreateDbContext();
        return db.Users.Single(u => u.Id == id);
    }

    private int AddHabit(string title = "Зарядка", Difficulty difficulty = Difficulty.Medium, string days = "1111111", int? userId = null)
    {
        var result = _service.Add(userId ?? _userId, title, 1, difficulty, days);
        Assert.True(result.IsValid);

        using var db = _database.CreateDbContext();
        return db.Habits.Single(h => h.Title == title).Id;
    }

    [Fact]
    public void Add_ValidHabit_Succeeds()
    {
        var result = _service.Add(_userId, "  Зарядка  ", 1, Difficulty.Easy, "1111111");

        Assert.True(result.IsValid);
        Assert.Equal("Зарядка", _service.GetHabits(_userId, Monday, false).Single().Title);
    }

    [Fact]
    public void Add_ShortTitle_Fails()
    {
        ValidationResult result = _service.Add(_userId, "ab", 1, Difficulty.Easy, "1111111");

        Assert.False(result.IsValid);
        Assert.Empty(_service.GetHabits(_userId, Monday, false));
    }

    [Fact]
    public void Add_UnknownCategory_Fails()
    {
        Assert.False(_service.Add(_userId, "Зарядка", 999, Difficulty.Easy, "1111111").IsValid);
    }

    [Fact]
    public void GetHabits_OnlyScheduled_FiltersByDayOfWeek()
    {
        AddHabit("Только понедельник", days: "1000000");
        AddHabit("Кроме понедельника", days: "0111111");

        var scheduled = _service.GetHabits(_userId, Monday, true);
        var all = _service.GetHabits(_userId, Monday, false);

        Assert.Equal("Только понедельник", scheduled.Single().Title);
        Assert.Equal(2, all.Count);
        Assert.Single(all, h => h.IsScheduled);
    }

    [Fact]
    public void MarkCompleted_AddsXpForDifficulty()
    {
        int habitId = AddHabit(difficulty: Difficulty.Hard);

        var result = _service.MarkCompleted(_userId, habitId, Monday);

        Assert.True(result.IsSuccess);
        Assert.Equal(30, result.XpEarned);
        Assert.Equal(30, GetUser(_userId).TotalXp);
        Assert.Equal(HabitStatus.Completed, _service.GetHabits(_userId, Monday, true).Single().Status);
    }

    [Fact]
    public void MarkCompleted_EnoughXp_RaisesLevel()
    {
        int userId = CreateUser("second", totalXp: 95);
        int habitId = AddHabit("Учить C#", Difficulty.Hard, userId: userId);

        var result = _service.MarkCompleted(userId, habitId, Monday);

        Assert.True(result.LeveledUp);
        Assert.Equal(2, result.Level);
        Assert.Equal(125, result.TotalXp);
        Assert.Equal(2, GetUser(userId).Level);
    }

    [Fact]
    public void MarkCompleted_Twice_FailsAndDoesNotAddXpAgain()
    {
        int habitId = AddHabit(difficulty: Difficulty.Medium);
        _service.MarkCompleted(_userId, habitId, Monday);

        var second = _service.MarkCompleted(_userId, habitId, Monday);

        Assert.False(second.IsSuccess);
        Assert.Equal(20, GetUser(_userId).TotalXp);
    }

    [Fact]
    public void MarkCompleted_OnNotScheduledDay_Fails()
    {
        int habitId = AddHabit(days: "1000000");

        Assert.False(_service.MarkCompleted(_userId, habitId, Tuesday).IsSuccess);
        Assert.Equal(0, GetUser(_userId).TotalXp);
    }

    [Fact]
    public void MarkCompleted_OtherUsersHabit_Fails()
    {
        int habitId = AddHabit();
        int otherUserId = CreateUser("other");

        Assert.False(_service.MarkCompleted(otherUserId, habitId, Monday).IsSuccess);
    }

    [Fact]
    public void MarkSkipped_GivesNoXpAndBlocksCompletion()
    {
        int habitId = AddHabit();

        var skipped = _service.MarkSkipped(_userId, habitId, Monday);
        var completed = _service.MarkCompleted(_userId, habitId, Monday);

        Assert.True(skipped.IsSuccess);
        Assert.False(completed.IsSuccess);
        Assert.Equal(0, GetUser(_userId).TotalXp);
        Assert.Equal(HabitStatus.Missed, _service.GetHabits(_userId, Monday, true).Single().Status);
    }

    [Fact]
    public void Update_ChangesFields()
    {
        int habitId = AddHabit();

        var result = _service.Update(_userId, habitId, "Бег", 2, Difficulty.Hard, "1010101");
        var habit = _service.GetHabit(_userId, habitId)!;

        Assert.True(result.IsValid);
        Assert.Equal("Бег", habit.Title);
        Assert.Equal(2, habit.CategoryId);
        Assert.Equal(Difficulty.Hard, habit.Difficulty);
        Assert.Equal("1010101", habit.DaysOfWeek);
    }

    [Fact]
    public void Delete_RemovesHabitAndItsLogs()
    {
        int habitId = AddHabit();
        _service.MarkCompleted(_userId, habitId, Monday);

        bool deleted = _service.Delete(_userId, habitId);

        Assert.True(deleted);
        Assert.Null(_service.GetHabit(_userId, habitId));
        using var db = _database.CreateDbContext();
        Assert.Empty(db.HabitLogs);
    }

    [Fact]
    public void Delete_UnknownHabit_ReturnsFalse()
    {
        Assert.False(_service.Delete(_userId, 9999));
    }
}

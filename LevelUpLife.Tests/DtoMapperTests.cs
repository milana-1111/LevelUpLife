using LevelUpLife.Core.Contracts;
using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Services;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Тесты преобразования моделей в объекты для передачи по сети.
/// </summary>
public class DtoMapperTests
{
    [Fact]
    public void UserDto_DoesNotContainPasswordHash()
    {
        var user = new User { Id = 1, Username = "milana", PasswordHash = "SECRET-HASH", DisplayName = "Милана", TotalXp = 40, Level = 1 };

        UserDto dto = DtoMapper.ToDto(user);

        Assert.DoesNotContain(typeof(UserDto).GetProperties(), p => p.Name.Contains("Password"));
        Assert.Equal("milana", dto.Username);
        Assert.Equal(40, dto.TotalXp);
    }

    [Fact]
    public void User_RoundTrip_KeepsData()
    {
        var user = new User { Id = 7, Username = "milana", DisplayName = "Милана", TotalXp = 120, Level = 2 };

        User restored = DtoMapper.ToUser(DtoMapper.ToDto(user));

        Assert.Equal(7, restored.Id);
        Assert.Equal("Милана", restored.DisplayName);
        Assert.Equal(120, restored.TotalXp);
        Assert.Equal(2, restored.Level);
        Assert.Equal(string.Empty, restored.PasswordHash);
    }

    [Fact]
    public void Habit_RoundTrip_KeepsData()
    {
        var habit = new Habit { Id = 3, Title = "Бег", CategoryId = 1, Difficulty = Difficulty.Hard, DaysOfWeek = "1010101" };

        Habit restored = DtoMapper.ToHabit(DtoMapper.ToDto(habit));

        Assert.Equal("Бег", restored.Title);
        Assert.Equal(Difficulty.Hard, restored.Difficulty);
        Assert.Equal("1010101", restored.DaysOfWeek);
    }

    [Fact]
    public void Completion_RoundTrip_KeepsData()
    {
        var result = CompletionResult.Success(30, 125, 2, true);

        CompletionResult restored = DtoMapper.ToCompletionResult(DtoMapper.ToDto(result));

        Assert.True(restored.IsSuccess);
        Assert.Equal(30, restored.XpEarned);
        Assert.Equal(2, restored.Level);
        Assert.True(restored.LeveledUp);
    }
}

using LevelUpLife.Core.Models;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Модульные тесты расписания привычки
/// </summary>
public class HabitTests
{
    // 16 ноября 2026 — понедельник, 21 — суббота, 22 — воскресенье
    [Theory]
    [InlineData("1111100", 2026, 11, 16, true)]
    [InlineData("1111100", 2026, 11, 21, false)]
    [InlineData("1111100", 2026, 11, 22, false)]
    [InlineData("0000001", 2026, 11, 22, true)]
    [InlineData("1111111", 2026, 11, 18, true)]
    public void IsScheduledFor_ChecksDayOfWeekMask(string mask, int year, int month, int day, bool expected)
    {
        var habit = new Habit { DaysOfWeek = mask };

        Assert.Equal(expected, habit.IsScheduledFor(new DateTime(year, month, day)));
    }

    [Fact]
    public void IsScheduledFor_InvalidMaskLength_ReturnsFalse()
    {
        var habit = new Habit { DaysOfWeek = "111" };

        Assert.False(habit.IsScheduledFor(new DateTime(2026, 11, 16)));
    }
}

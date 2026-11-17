using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Модульные тесты записи журнала выполнения привычки
/// </summary>
public class HabitLogTests
{
    [Fact]
    public void Complete_SetsStatusAndXp()
    {
        var log = new HabitLog();

        log.Complete(20);

        Assert.Equal(HabitStatus.Completed, log.Status);
        Assert.Equal(20, log.XpEarned);
    }

    [Fact]
    public void Complete_Twice_Throws()
    {
        var log = new HabitLog();
        log.Complete(10);

        Assert.Throws<InvalidOperationException>(() => log.Complete(10));
    }

    [Fact]
    public void Complete_NegativeXp_Throws()
    {
        var log = new HabitLog();

        Assert.Throws<ArgumentOutOfRangeException>(() => log.Complete(-5));
    }

    [Fact]
    public void Skip_SetsMissedAndZeroXp()
    {
        var log = new HabitLog { XpEarned = 15 };

        log.Skip();

        Assert.Equal(HabitStatus.Missed, log.Status);
        Assert.Equal(0, log.XpEarned);
    }

    [Fact]
    public void Skip_AfterComplete_Throws()
    {
        var log = new HabitLog();
        log.Complete(10);

        Assert.Throws<InvalidOperationException>(() => log.Skip());
    }
}

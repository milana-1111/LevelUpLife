using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.States;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Тесты паттерна «Состояние» для привычки за день.
/// </summary>
public class HabitStateTests
{
    [Theory]
    [InlineData(HabitStatus.Active, typeof(ActiveState))]
    [InlineData(HabitStatus.Completed, typeof(CompletedState))]
    [InlineData(HabitStatus.Missed, typeof(MissedState))]
    public void Factory_CreatesStateForStatus(HabitStatus status, Type expectedType)
    {
        Assert.IsType(expectedType, HabitStateFactory.Create(status));
    }

    [Fact]
    public void Factory_UnknownStatus_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => HabitStateFactory.Create((HabitStatus)99));
    }

    [Fact]
    public void ActiveState_CanComplete()
    {
        var log = new HabitLog();

        new ActiveState().Complete(log, 20);

        Assert.Equal(HabitStatus.Completed, log.Status);
        Assert.Equal(20, log.XpEarned);
    }

    [Fact]
    public void ActiveState_CanSkip()
    {
        var log = new HabitLog();

        new ActiveState().Skip(log);

        Assert.Equal(HabitStatus.Missed, log.Status);
    }

    [Fact]
    public void CompletedState_ForbidsAnyTransition()
    {
        var log = new HabitLog { Status = HabitStatus.Completed, XpEarned = 10 };
        var state = new CompletedState();

        Assert.Throws<InvalidOperationException>(() => state.Complete(log, 10));
        Assert.Throws<InvalidOperationException>(() => state.Skip(log));
        Assert.Equal(10, log.XpEarned);
    }

    [Fact]
    public void MissedState_ForbidsAnyTransition()
    {
        var log = new HabitLog { Status = HabitStatus.Missed };
        var state = new MissedState();

        Assert.Throws<InvalidOperationException>(() => state.Complete(log, 10));
        Assert.Throws<InvalidOperationException>(() => state.Skip(log));
    }

    [Fact]
    public void HabitLog_GetState_FollowsStatus()
    {
        var log = new HabitLog();
        Assert.IsType<ActiveState>(log.GetState());

        log.Complete(10);
        Assert.IsType<CompletedState>(log.GetState());
    }

    [Fact]
    public void HabitLog_MissedDay_CannotBeCompleted()
    {
        var log = new HabitLog();
        log.Skip();

        Assert.Throws<InvalidOperationException>(() => log.Complete(10));
    }
}

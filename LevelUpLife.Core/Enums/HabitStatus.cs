namespace LevelUpLife.Core.Enums;

/// <summary>
/// Состояние привычки за конкретный день.
/// </summary>
public enum HabitStatus
{
    /// <summary>Привычка запланирована на день и ещё не отмечена.</summary>
    Active = 0,

    /// <summary>Привычка выполнена.</summary>
    Completed = 1,

    /// <summary>Привычка пропущена.</summary>
    Missed = 2
}

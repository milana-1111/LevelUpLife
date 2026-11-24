using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.States;

/// <summary>
/// Создаёт объект состояния по значению статуса, которое хранится в базе данных.
/// </summary>
public static class HabitStateFactory
{
    /// <summary>
    /// Возвращает состояние, соответствующее статусу.
    /// </summary>
    /// <param name="status">Статус привычки за день.</param>
    /// <returns>Объект состояния.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Если статус неизвестен.</exception>
    public static IHabitState Create(HabitStatus status)
    {
        return status switch
        {
            HabitStatus.Active => new ActiveState(),
            HabitStatus.Completed => new CompletedState(),
            HabitStatus.Missed => new MissedState(),
            _ => throw new ArgumentOutOfRangeException(nameof(status), "Неизвестный статус привычки.")
        };
    }
}

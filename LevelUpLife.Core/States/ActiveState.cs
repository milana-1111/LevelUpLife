using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;

namespace LevelUpLife.Core.States;

/// <summary>
/// Состояние «Активна»: привычка запланирована на день и ещё не отмечена.
/// Из него можно перейти и в «Выполнена», и в «Пропущена».
/// </summary>
public class ActiveState : IHabitState
{
    /// <summary>Соответствующий статус — «Активна».</summary>
    public HabitStatus Status => HabitStatus.Active;

    /// <summary>
    /// Отмечает привычку выполненной и записывает опыт.
    /// </summary>
    /// <param name="log">Запись журнала.</param>
    /// <param name="xp">Начисленный опыт.</param>
    public void Complete(HabitLog log, int xp)
    {
        log.Status = HabitStatus.Completed;
        log.XpEarned = xp;
    }

    /// <summary>
    /// Отмечает привычку пропущенной, опыт обнуляется.
    /// </summary>
    /// <param name="log">Запись журнала.</param>
    public void Skip(HabitLog log)
    {
        log.Status = HabitStatus.Missed;
        log.XpEarned = 0;
    }
}

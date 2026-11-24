using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;

namespace LevelUpLife.Core.States;

/// <summary>
/// Состояние «Выполнена»: день закрыт, изменить его нельзя.
/// </summary>
public class CompletedState : IHabitState
{
    /// <summary>Соответствующий статус — «Выполнена».</summary>
    public HabitStatus Status => HabitStatus.Completed;

    /// <summary>
    /// Повторная отметка запрещена.
    /// </summary>
    /// <param name="log">Запись журнала.</param>
    /// <param name="xp">Начисленный опыт (не используется).</param>
    /// <exception cref="InvalidOperationException">Всегда: привычка уже выполнена.</exception>
    public void Complete(HabitLog log, int xp)
    {
        throw new InvalidOperationException("Привычка уже отмечена выполненной за этот день.");
    }

    /// <summary>
    /// Пропуск выполненной привычки запрещён.
    /// </summary>
    /// <param name="log">Запись журнала.</param>
    /// <exception cref="InvalidOperationException">Всегда: привычка уже выполнена.</exception>
    public void Skip(HabitLog log)
    {
        throw new InvalidOperationException("Нельзя пропустить уже выполненную привычку.");
    }
}

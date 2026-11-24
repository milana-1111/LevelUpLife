using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;

namespace LevelUpLife.Core.States;

/// <summary>
/// Состояние «Пропущена»: день закрыт, изменить его нельзя.
/// </summary>
public class MissedState : IHabitState
{
    /// <summary>Соответствующий статус — «Пропущена».</summary>
    public HabitStatus Status => HabitStatus.Missed;

    /// <summary>
    /// Отметка выполненной для пропущенного дня запрещена.
    /// </summary>
    /// <param name="log">Запись журнала.</param>
    /// <param name="xp">Начисленный опыт (не используется).</param>
    /// <exception cref="InvalidOperationException">Всегда: день уже пропущен.</exception>
    public void Complete(HabitLog log, int xp)
    {
        throw new InvalidOperationException("Этот день уже отмечен как пропущенный.");
    }

    /// <summary>
    /// Повторный пропуск запрещён.
    /// </summary>
    /// <param name="log">Запись журнала.</param>
    /// <exception cref="InvalidOperationException">Всегда: день уже пропущен.</exception>
    public void Skip(HabitLog log)
    {
        throw new InvalidOperationException("Этот день уже отмечен как пропущенный.");
    }
}

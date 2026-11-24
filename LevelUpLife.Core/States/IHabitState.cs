using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;

namespace LevelUpLife.Core.States;

/// <summary>
/// Состояние привычки за день (паттерн «Состояние»).
/// Каждое состояние само решает, какие переходы допустимы.
/// </summary>
public interface IHabitState
{
    /// <summary>Какому значению <see cref="HabitStatus"/> соответствует это состояние.</summary>
    HabitStatus Status { get; }

    /// <summary>
    /// Переводит запись журнала в состояние «Выполнена».
    /// </summary>
    /// <param name="log">Запись журнала, состояние которой меняется.</param>
    /// <param name="xp">Начисленный опыт.</param>
    /// <exception cref="InvalidOperationException">Если из текущего состояния такой переход запрещён.</exception>
    void Complete(HabitLog log, int xp);

    /// <summary>
    /// Переводит запись журнала в состояние «Пропущена».
    /// </summary>
    /// <param name="log">Запись журнала, состояние которой меняется.</param>
    /// <exception cref="InvalidOperationException">Если из текущего состояния такой переход запрещён.</exception>
    void Skip(HabitLog log);
}

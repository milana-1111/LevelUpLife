using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.Models;

/// <summary>
/// Запись журнала: состояние одной привычки за один день.
/// Пара «привычка + дата» уникальна.
/// </summary>
public class HabitLog
{
    /// <summary>Идентификатор записи.</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор привычки.</summary>
    public int HabitId { get; set; }

    /// <summary>Дата, за которую сделана отметка.</summary>
    public DateTime LogDate { get; set; }

    /// <summary>Состояние привычки за этот день.</summary>
    public HabitStatus Status { get; set; } = HabitStatus.Active;

    /// <summary>Сколько XP начислено за этот день.</summary>
    public int XpEarned { get; set; }

    /// <summary>Привычка, к которой относится запись.</summary>
    public Habit? Habit { get; set; }

    /// <summary>
    /// Отмечает привычку выполненной и запоминает начисленный опыт.
    /// </summary>
    /// <param name="xp">Начисленное XP. Не может быть отрицательным.</param>
    /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="xp"/> меньше нуля.</exception>
    /// <exception cref="InvalidOperationException">Если привычка уже отмечена выполненной.</exception>
    public void Complete(int xp)
    {
        if (xp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(xp), "Количество XP не может быть отрицательным.");
        }

        if (Status == HabitStatus.Completed)
        {
            throw new InvalidOperationException("Привычка уже отмечена выполненной за этот день.");
        }

        Status = HabitStatus.Completed;
        XpEarned = xp;
    }

    /// <summary>
    /// Отмечает привычку пропущенной. Опыт за такой день не начисляется.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если привычка уже выполнена.</exception>
    public void Skip()
    {
        if (Status == HabitStatus.Completed)
        {
            throw new InvalidOperationException("Нельзя пропустить уже выполненную привычку.");
        }

        Status = HabitStatus.Missed;
        XpEarned = 0;
    }
}

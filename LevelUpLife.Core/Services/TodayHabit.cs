using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.Services;

/// <summary>
/// Привычка со статусом за выбранный день. Нужна для списка на главном экране.
/// </summary>
public class TodayHabit
{
    /// <summary>Идентификатор привычки.</summary>
    public int HabitId { get; set; }

    /// <summary>Название привычки.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Название категории.</summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Сложность привычки.</summary>
    public Difficulty Difficulty { get; set; }

    /// <summary>Состояние привычки за выбранный день.</summary>
    public HabitStatus Status { get; set; }

    /// <summary>Сколько XP начислено за выбранный день.</summary>
    public int XpEarned { get; set; }

    /// <summary>Запланирована ли привычка на выбранный день недели.</summary>
    public bool IsScheduled { get; set; }
}

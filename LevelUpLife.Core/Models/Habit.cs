using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.Models;

/// <summary>
/// Привычка пользователя.
/// </summary>
public class Habit
{
    /// <summary>Идентификатор привычки.</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор владельца привычки.</summary>
    public int UserId { get; set; }

    /// <summary>Идентификатор категории привычки.</summary>
    public int CategoryId { get; set; }

    /// <summary>Название привычки (от 3 до 100 символов).</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Сложность привычки.</summary>
    public Difficulty Difficulty { get; set; } = Difficulty.Easy;

    /// <summary>
    /// Дни недели, в которые нужно выполнять привычку: 7 символов «0» или «1»,
    /// по порядку с понедельника по воскресенье. Например, «1111100» — по будням.
    /// </summary>
    public string DaysOfWeek { get; set; } = "1111111";

    /// <summary>Владелец привычки.</summary>
    public User? User { get; set; }

    /// <summary>Категория привычки.</summary>
    public Category? Category { get; set; }

    /// <summary>Записи о выполнении привычки по дням.</summary>
    public List<HabitLog> Logs { get; set; } = new();

    /// <summary>
    /// Проверяет, запланирована ли привычка на указанный день.
    /// </summary>
    /// <param name="date">Дата, для которой выполняется проверка.</param>
    /// <returns><c>true</c>, если в этот день недели привычку нужно выполнять; иначе <c>false</c>.</returns>
    public bool IsScheduledFor(DateTime date)
    {
        if (DaysOfWeek.Length != 7)
        {
            return false;
        }

        // DayOfWeek считает с воскресенья (0), а в маске понедельник идёт первым.
        int index = ((int)date.DayOfWeek + 6) % 7;
        return DaysOfWeek[index] == '1';
    }
}

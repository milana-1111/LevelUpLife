namespace LevelUpLife.Core.Services;

/// <summary>
/// Строка таблицы лидеров.
/// </summary>
public class LeaderboardEntry
{
    /// <summary>Место в рейтинге (у игроков с равным опытом место одинаковое).</summary>
    public int Rank { get; set; }

    /// <summary>Идентификатор пользователя.</summary>
    public int UserId { get; set; }

    /// <summary>Имя в рейтинге.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Уровень игрока.</summary>
    public int Level { get; set; }

    /// <summary>Весь накопленный опыт.</summary>
    public int TotalXp { get; set; }
}

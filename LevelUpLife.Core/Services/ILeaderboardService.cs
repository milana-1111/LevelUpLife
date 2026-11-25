namespace LevelUpLife.Core.Services;

/// <summary>
/// Таблица лидеров: рейтинг пользователей по накопленному опыту.
/// </summary>
public interface ILeaderboardService
{
    /// <summary>
    /// Возвращает лучших игроков.
    /// </summary>
    /// <param name="count">Сколько игроков вернуть (больше нуля).</param>
    /// <returns>Список по убыванию опыта.</returns>
    List<LeaderboardEntry> GetTop(int count);

    /// <summary>
    /// Возвращает место пользователя в рейтинге.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Место в рейтинге или 0, если пользователь не найден.</returns>
    int GetRank(int userId);
}

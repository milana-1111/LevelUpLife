using LevelUpLife.Core.Data;
using LevelUpLife.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLife.Core.Services;

/// <summary>
/// Таблица лидеров на основе данных всех пользователей в базе.
/// </summary>
public class LeaderboardService : ILeaderboardService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    /// <summary>
    /// Создаёт сервис рейтинга.
    /// </summary>
    /// <param name="dbFactory">Фабрика контекстов базы данных.</param>
    public LeaderboardService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Возвращает лучших игроков. Игроки с равным опытом получают одинаковое место.
    /// </summary>
    /// <param name="count">Сколько игроков вернуть (больше нуля).</param>
    /// <returns>Список по убыванию опыта.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="count"/> меньше 1.</exception>
    public List<LeaderboardEntry> GetTop(int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Количество игроков должно быть больше нуля.");
        }

        using AppDbContext db = _dbFactory.CreateDbContext();
        List<User> users = db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.TotalXp)
            .ThenBy(u => u.DisplayName)
            .Take(count)
            .ToList();

        var result = new List<LeaderboardEntry>();
        for (int i = 0; i < users.Count; i++)
        {
            bool sameAsPrevious = i > 0 && users[i].TotalXp == users[i - 1].TotalXp;
            result.Add(new LeaderboardEntry
            {
                Rank = sameAsPrevious ? result[i - 1].Rank : i + 1,
                UserId = users[i].Id,
                DisplayName = users[i].DisplayName,
                Level = users[i].Level,
                TotalXp = users[i].TotalXp
            });
        }

        return result;
    }

    /// <summary>
    /// Возвращает место пользователя в рейтинге.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Место в рейтинге или 0, если пользователь не найден.</returns>
    public int GetRank(int userId)
    {
        using AppDbContext db = _dbFactory.CreateDbContext();
        User? user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return 0;
        }

        int xp = user.TotalXp;
        return 1 + db.Users.Count(u => u.TotalXp > xp);
    }
}

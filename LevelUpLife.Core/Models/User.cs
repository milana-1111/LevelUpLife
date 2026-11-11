namespace LevelUpLife.Core.Models;

/// <summary>
/// Пользователь (игрок) приложения.
/// </summary>
public class User
{
    /// <summary>Идентификатор пользователя.</summary>
    public int Id { get; set; }

    /// <summary>Логин. Уникален в системе.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Хеш пароля. Сам пароль нигде не хранится.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Имя, которое показывается в таблице лидеров.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Весь накопленный опыт (XP).</summary>
    public int TotalXp { get; set; }

    /// <summary>Текущий уровень.</summary>
    public int Level { get; set; } = 1;

    /// <summary>Сессии пользователя.</summary>
    public List<Session> Sessions { get; set; } = new();

    /// <summary>Привычки пользователя.</summary>
    public List<Habit> Habits { get; set; } = new();

    /// <summary>
    /// Добавляет пользователю опыт.
    /// </summary>
    /// <param name="xp">Количество начисляемого XP. Не может быть отрицательным.</param>
    /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="xp"/> меньше нуля.</exception>
    public void AddXp(int xp)
    {
        if (xp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(xp), "Количество XP не может быть отрицательным.");
        }

        TotalXp += xp;
    }
}

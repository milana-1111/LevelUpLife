namespace LevelUpLife.Core.Models;

/// <summary>
/// Сессия авторизации. Нужна, чтобы после перезапуска приложения
/// не вводить пароль заново.
/// </summary>
public class Session
{
    /// <summary>Идентификатор сессии.</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор владельца сессии.</summary>
    public int UserId { get; set; }

    /// <summary>Токен сессии. Уникален.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Время, до которого сессия действует (UTC).</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Владелец сессии.</summary>
    public User? User { get; set; }

    /// <summary>
    /// Проверяет, истёк ли срок действия сессии.
    /// </summary>
    /// <returns><c>true</c>, если срок действия сессии закончился; иначе <c>false</c>.</returns>
    public bool IsExpired()
    {
        return ExpiresAt <= DateTime.UtcNow;
    }
}

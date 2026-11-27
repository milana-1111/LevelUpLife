namespace LevelUpLife.Core.Services;

/// <summary>
/// Проверка токена сессии на сервере: по токену из заголовка запроса определяется пользователь.
/// </summary>
public interface ITokenAuthenticator
{
    /// <summary>
    /// Проверяет токен и возвращает пользователя, которому он принадлежит.
    /// </summary>
    /// <param name="token">Токен сессии.</param>
    /// <returns>Результат проверки. При успехе содержит пользователя.</returns>
    AuthResult Authenticate(string token);

    /// <summary>
    /// Отзывает токен: сессия удаляется, и токен перестаёт работать.
    /// </summary>
    /// <param name="token">Токен сессии.</param>
    void Revoke(string token);
}

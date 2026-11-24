namespace LevelUpLife.Core.Services;

/// <summary>
/// Хранилище токена сессии на компьютере пользователя.
/// Благодаря ему после перезапуска приложения не нужно вводить пароль заново.
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// Читает сохранённый токен.
    /// </summary>
    /// <returns>Токен или <c>null</c>, если он не сохранён.</returns>
    string? Load();

    /// <summary>
    /// Сохраняет токен сессии.
    /// </summary>
    /// <param name="token">Токен сессии.</param>
    void Save(string token);

    /// <summary>
    /// Удаляет сохранённый токен.
    /// </summary>
    void Clear();
}

using LevelUpLife.Core.Models;

namespace LevelUpLife.Core.Services;

/// <summary>
/// Регистрация, вход и сессии пользователей.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрирует нового пользователя и сразу входит в систему.
    /// </summary>
    /// <param name="username">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="displayName">Имя в таблице лидеров.</param>
    /// <returns>Результат операции.</returns>
    AuthResult Register(string username, string password, string displayName);

    /// <summary>
    /// Выполняет вход по логину и паролю.
    /// </summary>
    /// <param name="username">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Результат операции.</returns>
    AuthResult Login(string username, string password);

    /// <summary>
    /// Завершает текущую сессию и удаляет сохранённый токен.
    /// </summary>
    void Logout();

    /// <summary>
    /// Восстанавливает вход по сохранённому токену (после перезапуска приложения).
    /// </summary>
    /// <returns>Результат операции.</returns>
    AuthResult RestoreSession();

    /// <summary>
    /// Загружает актуальные данные пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Пользователь или <c>null</c>, если не найден.</returns>
    User? GetUser(int userId);
}

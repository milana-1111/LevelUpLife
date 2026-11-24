using LevelUpLife.Core.Models;

namespace LevelUpLife.Core.Services;

/// <summary>
/// Результат входа, регистрации или восстановления сессии.
/// </summary>
public class AuthResult
{
    /// <summary>Признак успешного выполнения операции.</summary>
    public bool IsSuccess { get; private set; }

    /// <summary>Пользователь (заполнен только при успехе).</summary>
    public User? User { get; private set; }

    /// <summary>Сообщение об ошибке для пользователя (заполнено только при неудаче).</summary>
    public string ErrorMessage { get; private set; } = string.Empty;

    /// <summary>
    /// Создаёт успешный результат.
    /// </summary>
    /// <param name="user">Пользователь, который вошёл в систему.</param>
    /// <returns>Успешный результат.</returns>
    public static AuthResult Success(User user)
    {
        return new AuthResult { IsSuccess = true, User = user };
    }

    /// <summary>
    /// Создаёт результат с ошибкой.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Результат с ошибкой.</returns>
    public static AuthResult Fail(string message)
    {
        return new AuthResult { IsSuccess = false, ErrorMessage = message };
    }
}

namespace LevelUpLife.Core.Services;

/// <summary>
/// Результат отметки привычки: сколько опыта получено и какой теперь уровень.
/// </summary>
public class CompletionResult
{
    /// <summary>Признак успешного выполнения операции.</summary>
    public bool IsSuccess { get; private set; }

    /// <summary>Сообщение об ошибке (заполнено только при неудаче).</summary>
    public string ErrorMessage { get; private set; } = string.Empty;

    /// <summary>Начисленный опыт.</summary>
    public int XpEarned { get; private set; }

    /// <summary>Весь опыт пользователя после операции.</summary>
    public int TotalXp { get; private set; }

    /// <summary>Уровень пользователя после операции.</summary>
    public int Level { get; private set; }

    /// <summary>Повысился ли уровень в результате операции.</summary>
    public bool LeveledUp { get; private set; }

    /// <summary>
    /// Создаёт успешный результат.
    /// </summary>
    /// <param name="xpEarned">Начисленный опыт.</param>
    /// <param name="totalXp">Весь опыт пользователя.</param>
    /// <param name="level">Текущий уровень.</param>
    /// <param name="leveledUp">Повысился ли уровень.</param>
    /// <returns>Успешный результат.</returns>
    public static CompletionResult Success(int xpEarned, int totalXp, int level, bool leveledUp)
    {
        return new CompletionResult
        {
            IsSuccess = true,
            XpEarned = xpEarned,
            TotalXp = totalXp,
            Level = level,
            LeveledUp = leveledUp
        };
    }

    /// <summary>
    /// Создаёт результат с ошибкой.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Результат с ошибкой.</returns>
    public static CompletionResult Fail(string message)
    {
        return new CompletionResult { IsSuccess = false, ErrorMessage = message };
    }
}

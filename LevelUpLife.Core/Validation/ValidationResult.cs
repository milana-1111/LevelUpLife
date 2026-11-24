namespace LevelUpLife.Core.Validation;

/// <summary>
/// Результат проверки введённых данных: список ошибок. Если ошибок нет — данные корректны.
/// </summary>
public class ValidationResult
{
    private readonly List<string> _errors = new();

    /// <summary>Список найденных ошибок.</summary>
    public IReadOnlyList<string> Errors => _errors;

    /// <summary>Признак того, что данные корректны (ошибок нет).</summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>Все ошибки одной строкой, каждая с новой строки.</summary>
    public string ErrorsText => string.Join(Environment.NewLine, _errors);

    /// <summary>
    /// Добавляет ошибку в результат проверки.
    /// </summary>
    /// <param name="message">Текст ошибки для пользователя.</param>
    public void AddError(string message)
    {
        _errors.Add(message);
    }

    /// <summary>
    /// Создаёт результат без ошибок.
    /// </summary>
    /// <returns>Успешный результат проверки.</returns>
    public static ValidationResult Success()
    {
        return new ValidationResult();
    }

    /// <summary>
    /// Создаёт результат с одной ошибкой.
    /// </summary>
    /// <param name="message">Текст ошибки для пользователя.</param>
    /// <returns>Результат проверки с ошибкой.</returns>
    public static ValidationResult Failure(string message)
    {
        var result = new ValidationResult();
        result.AddError(message);
        return result;
    }
}

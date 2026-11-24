using System.Text.RegularExpressions;
using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.Validation;

/// <summary>
/// Проверка данных, которые вводит пользователь: логин, пароль, имя, параметры привычки.
/// </summary>
public class InputValidator
{
    private static readonly Regex UsernameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

    /// <summary>
    /// Проверяет данные для входа в систему.
    /// </summary>
    /// <param name="username">Введённый логин.</param>
    /// <param name="password">Введённый пароль.</param>
    /// <returns>Результат проверки со списком ошибок.</returns>
    public ValidationResult ValidateLogin(string? username, string? password)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(username))
        {
            result.AddError("Введите логин.");
        }

        if (string.IsNullOrEmpty(password))
        {
            result.AddError("Введите пароль.");
        }

        return result;
    }

    /// <summary>
    /// Проверяет данные для регистрации нового пользователя.
    /// </summary>
    /// <param name="username">Логин: 3–30 символов, латинские буквы, цифры и «_».</param>
    /// <param name="password">Пароль: 6–64 символа.</param>
    /// <param name="displayName">Имя в рейтинге: 2–30 символов.</param>
    /// <returns>Результат проверки со списком ошибок.</returns>
    public ValidationResult ValidateRegistration(string? username, string? password, string? displayName)
    {
        var result = new ValidationResult();

        string login = (username ?? string.Empty).Trim();
        if (login.Length < 3 || login.Length > 30)
        {
            result.AddError("Логин должен содержать от 3 до 30 символов.");
        }
        else if (!UsernameRegex.IsMatch(login))
        {
            result.AddError("Логин может содержать только латинские буквы, цифры и знак «_».");
        }

        int passwordLength = password?.Length ?? 0;
        if (passwordLength < 6 || passwordLength > 64)
        {
            result.AddError("Пароль должен содержать от 6 до 64 символов.");
        }

        string name = (displayName ?? string.Empty).Trim();
        if (name.Length < 2 || name.Length > 30)
        {
            result.AddError("Имя в рейтинге должно содержать от 2 до 30 символов.");
        }

        return result;
    }

    /// <summary>
    /// Проверяет параметры привычки при создании и редактировании.
    /// </summary>
    /// <param name="title">Название: 3–100 символов.</param>
    /// <param name="categoryId">Идентификатор выбранной категории (больше нуля).</param>
    /// <param name="difficulty">Выбранная сложность.</param>
    /// <param name="daysOfWeek">Маска дней недели: 7 символов «0»/«1», хотя бы одна «1».</param>
    /// <returns>Результат проверки со списком ошибок.</returns>
    public ValidationResult ValidateHabit(string? title, int categoryId, Difficulty difficulty, string? daysOfWeek)
    {
        var result = new ValidationResult();

        string habitTitle = (title ?? string.Empty).Trim();
        if (habitTitle.Length < 3 || habitTitle.Length > 100)
        {
            result.AddError("Название привычки должно содержать от 3 до 100 символов.");
        }

        if (categoryId <= 0)
        {
            result.AddError("Выберите категорию.");
        }

        if (!Enum.IsDefined(difficulty))
        {
            result.AddError("Выберите сложность.");
        }

        string days = daysOfWeek ?? string.Empty;
        bool maskCorrect = days.Length == 7 && days.All(c => c == '0' || c == '1');
        if (!maskCorrect || !days.Contains('1'))
        {
            result.AddError("Выберите хотя бы один день недели.");
        }

        return result;
    }
}

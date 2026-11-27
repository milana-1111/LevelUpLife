using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.Contracts;

/// <summary>
/// Запрос на регистрацию нового пользователя.
/// </summary>
/// <param name="Username">Логин.</param>
/// <param name="Password">Пароль.</param>
/// <param name="DisplayName">Имя в таблице лидеров.</param>
public record RegisterRequest(string Username, string Password, string DisplayName);

/// <summary>
/// Запрос на вход.
/// </summary>
/// <param name="Username">Логин.</param>
/// <param name="Password">Пароль.</param>
public record LoginRequest(string Username, string Password);

/// <summary>
/// Данные пользователя, которые безопасно отдавать клиенту (без хеша пароля).
/// </summary>
/// <param name="Id">Идентификатор пользователя.</param>
/// <param name="Username">Логин.</param>
/// <param name="DisplayName">Имя в таблице лидеров.</param>
/// <param name="TotalXp">Весь накопленный опыт.</param>
/// <param name="Level">Текущий уровень.</param>
public record UserDto(int Id, string Username, string DisplayName, int TotalXp, int Level);

/// <summary>
/// Ответ сервера на успешный вход или регистрацию.
/// </summary>
/// <param name="Token">Токен сессии. Клиент присылает его в заголовке Authorization.</param>
/// <param name="User">Данные пользователя.</param>
public record AuthResponse(string Token, UserDto User);

/// <summary>
/// Категория привычек.
/// </summary>
/// <param name="Id">Идентификатор категории.</param>
/// <param name="Name">Название категории.</param>
public record CategoryDto(int Id, string Name);

/// <summary>
/// Привычка пользователя.
/// </summary>
/// <param name="Id">Идентификатор привычки.</param>
/// <param name="Title">Название.</param>
/// <param name="CategoryId">Идентификатор категории.</param>
/// <param name="Difficulty">Сложность.</param>
/// <param name="DaysOfWeek">Маска дней недели из 7 символов «0»/«1».</param>
public record HabitDto(int Id, string Title, int CategoryId, Difficulty Difficulty, string DaysOfWeek);

/// <summary>
/// Запрос на создание или изменение привычки.
/// </summary>
/// <param name="Title">Название.</param>
/// <param name="CategoryId">Идентификатор категории.</param>
/// <param name="Difficulty">Сложность.</param>
/// <param name="DaysOfWeek">Маска дней недели из 7 символов «0»/«1».</param>
public record HabitRequest(string Title, int CategoryId, Difficulty Difficulty, string DaysOfWeek);

/// <summary>
/// Результат успешной отметки привычки.
/// </summary>
/// <param name="XpEarned">Начисленный опыт.</param>
/// <param name="TotalXp">Весь опыт пользователя после отметки.</param>
/// <param name="Level">Уровень пользователя после отметки.</param>
/// <param name="LeveledUp">Повысился ли уровень.</param>
public record CompletionDto(int XpEarned, int TotalXp, int Level, bool LeveledUp);

/// <summary>
/// Место пользователя в рейтинге.
/// </summary>
/// <param name="Rank">Место в рейтинге (0, если пользователь не найден).</param>
public record RankDto(int Rank);

/// <summary>
/// Сообщение об ошибке, которое сервер присылает вместе с кодом ошибки.
/// </summary>
/// <param name="Error">Текст ошибки для пользователя.</param>
public record ErrorResponse(string Error);

using LevelUpLife.Core.Models;
using LevelUpLife.Core.Services;

namespace LevelUpLife.Core.Contracts;

/// <summary>
/// Преобразование моделей в объекты для передачи по сети и обратно.
/// Хеш пароля в передаваемые данные никогда не попадает.
/// </summary>
public static class DtoMapper
{
    /// <summary>
    /// Превращает пользователя в безопасный для передачи объект.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <returns>Данные пользователя без хеша пароля.</returns>
    public static UserDto ToDto(User user)
    {
        return new UserDto(user.Id, user.Username, user.DisplayName, user.TotalXp, user.Level);
    }

    /// <summary>
    /// Превращает категорию в объект для передачи.
    /// </summary>
    /// <param name="category">Категория.</param>
    /// <returns>Данные категории.</returns>
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto(category.Id, category.Name);
    }

    /// <summary>
    /// Превращает привычку в объект для передачи.
    /// </summary>
    /// <param name="habit">Привычка.</param>
    /// <returns>Данные привычки.</returns>
    public static HabitDto ToDto(Habit habit)
    {
        return new HabitDto(habit.Id, habit.Title, habit.CategoryId, habit.Difficulty, habit.DaysOfWeek);
    }

    /// <summary>
    /// Превращает успешный результат отметки в объект для передачи.
    /// </summary>
    /// <param name="result">Результат отметки привычки.</param>
    /// <returns>Данные об опыте и уровне.</returns>
    public static CompletionDto ToDto(CompletionResult result)
    {
        return new CompletionDto(result.XpEarned, result.TotalXp, result.Level, result.LeveledUp);
    }

    /// <summary>
    /// Восстанавливает пользователя из переданных данных.
    /// </summary>
    /// <param name="dto">Данные пользователя.</param>
    /// <returns>Пользователь (хеш пароля пустой).</returns>
    public static User ToUser(UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            DisplayName = dto.DisplayName,
            TotalXp = dto.TotalXp,
            Level = dto.Level
        };
    }

    /// <summary>
    /// Восстанавливает категорию из переданных данных.
    /// </summary>
    /// <param name="dto">Данные категории.</param>
    /// <returns>Категория.</returns>
    public static Category ToCategory(CategoryDto dto)
    {
        return new Category { Id = dto.Id, Name = dto.Name };
    }

    /// <summary>
    /// Восстанавливает привычку из переданных данных.
    /// </summary>
    /// <param name="dto">Данные привычки.</param>
    /// <returns>Привычка.</returns>
    public static Habit ToHabit(HabitDto dto)
    {
        return new Habit
        {
            Id = dto.Id,
            Title = dto.Title,
            CategoryId = dto.CategoryId,
            Difficulty = dto.Difficulty,
            DaysOfWeek = dto.DaysOfWeek
        };
    }

    /// <summary>
    /// Восстанавливает успешный результат отметки из переданных данных.
    /// </summary>
    /// <param name="dto">Данные об опыте и уровне.</param>
    /// <returns>Успешный результат отметки.</returns>
    public static CompletionResult ToCompletionResult(CompletionDto dto)
    {
        return CompletionResult.Success(dto.XpEarned, dto.TotalXp, dto.Level, dto.LeveledUp);
    }
}

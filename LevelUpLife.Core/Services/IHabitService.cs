using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Validation;

namespace LevelUpLife.Core.Services;

/// <summary>
/// Единая точка работы с привычками (паттерн «Фасад»).
/// Окна и ViewModel вызывают только этот интерфейс и ничего не знают
/// о базе данных, расчёте опыта и состояниях.
/// </summary>
public interface IHabitService
{
    /// <summary>
    /// Возвращает все категории.
    /// </summary>
    /// <returns>Список категорий по алфавиту.</returns>
    List<Category> GetCategories();

    /// <summary>
    /// Возвращает привычку пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <returns>Привычка или <c>null</c>, если не найдена.</returns>
    Habit? GetHabit(int userId, int habitId);

    /// <summary>
    /// Возвращает привычки пользователя со статусом за указанный день.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="date">День, за который нужен статус.</param>
    /// <param name="onlyScheduled">Показывать только привычки, запланированные на этот день недели.</param>
    /// <returns>Список привычек со статусами.</returns>
    List<TodayHabit> GetHabits(int userId, DateTime date, bool onlyScheduled);

    /// <summary>
    /// Создаёт привычку.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="title">Название.</param>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="difficulty">Сложность.</param>
    /// <param name="daysOfWeek">Маска дней недели из 7 символов «0»/«1».</param>
    /// <returns>Результат проверки: список ошибок или успех.</returns>
    ValidationResult Add(int userId, string title, int categoryId, Difficulty difficulty, string daysOfWeek);

    /// <summary>
    /// Изменяет привычку.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="title">Новое название.</param>
    /// <param name="categoryId">Новая категория.</param>
    /// <param name="difficulty">Новая сложность.</param>
    /// <param name="daysOfWeek">Новая маска дней недели.</param>
    /// <returns>Результат проверки: список ошибок или успех.</returns>
    ValidationResult Update(int userId, int habitId, string title, int categoryId, Difficulty difficulty, string daysOfWeek);

    /// <summary>
    /// Удаляет привычку вместе с её журналом.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <returns><c>true</c>, если привычка была удалена.</returns>
    bool Delete(int userId, int habitId);

    /// <summary>
    /// Отмечает привычку выполненной, начисляет опыт и пересчитывает уровень.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="date">День отметки.</param>
    /// <returns>Результат: полученный опыт и новый уровень либо ошибка.</returns>
    CompletionResult MarkCompleted(int userId, int habitId, DateTime date);

    /// <summary>
    /// Отмечает привычку пропущенной.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="date">День отметки.</param>
    /// <returns>Результат операции.</returns>
    CompletionResult MarkSkipped(int userId, int habitId, DateTime date);
}

using LevelUpLife.Core.Data;
using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Validation;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLife.Core.Services;

/// <summary>
/// Фасад над работой с привычками: объединяет проверку данных, базу данных,
/// расчёт опыта и состояния привычки в несколько простых методов.
/// </summary>
public class HabitService : IHabitService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly XpCalculator _xpCalculator;
    private readonly InputValidator _validator;

    /// <summary>
    /// Создаёт сервис привычек.
    /// </summary>
    /// <param name="dbFactory">Фабрика контекстов базы данных.</param>
    /// <param name="xpCalculator">Расчёт опыта и уровней.</param>
    /// <param name="validator">Проверка вводимых данных.</param>
    public HabitService(IDbContextFactory<AppDbContext> dbFactory, XpCalculator xpCalculator, InputValidator validator)
    {
        _dbFactory = dbFactory;
        _xpCalculator = xpCalculator;
        _validator = validator;
    }

    /// <summary>
    /// Возвращает все категории.
    /// </summary>
    /// <returns>Список категорий по алфавиту.</returns>
    public List<Category> GetCategories()
    {
        using AppDbContext db = _dbFactory.CreateDbContext();
        return db.Categories.AsNoTracking().OrderBy(c => c.Name).ToList();
    }

    /// <summary>
    /// Возвращает привычку пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <returns>Привычка или <c>null</c>, если не найдена.</returns>
    public Habit? GetHabit(int userId, int habitId)
    {
        using AppDbContext db = _dbFactory.CreateDbContext();
        return db.Habits.AsNoTracking().FirstOrDefault(h => h.Id == habitId && h.UserId == userId);
    }

    /// <summary>
    /// Возвращает привычки пользователя со статусом за указанный день.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="date">День, за который нужен статус.</param>
    /// <param name="onlyScheduled">Показывать только привычки, запланированные на этот день недели.</param>
    /// <returns>Список привычек со статусами.</returns>
    public List<TodayHabit> GetHabits(int userId, DateTime date, bool onlyScheduled)
    {
        DateTime day = date.Date;

        using AppDbContext db = _dbFactory.CreateDbContext();
        List<Habit> habits = db.Habits
            .AsNoTracking()
            .Include(h => h.Category)
            .Where(h => h.UserId == userId)
            .ToList();

        if (onlyScheduled)
        {
            habits = habits.Where(h => h.IsScheduledFor(day)).ToList();
        }

        List<int> ids = habits.Select(h => h.Id).ToList();
        List<HabitLog> logs = db.HabitLogs
            .AsNoTracking()
            .Where(l => ids.Contains(l.HabitId) && l.LogDate == day)
            .ToList();

        return habits
            .OrderBy(h => h.Title)
            .Select(h =>
            {
                HabitLog? log = logs.FirstOrDefault(l => l.HabitId == h.Id);
                return new TodayHabit
                {
                    HabitId = h.Id,
                    Title = h.Title,
                    CategoryName = h.Category?.Name ?? string.Empty,
                    Difficulty = h.Difficulty,
                    Status = log?.Status ?? HabitStatus.Active,
                    XpEarned = log?.XpEarned ?? 0,
                    IsScheduled = h.IsScheduledFor(day)
                };
            })
            .ToList();
    }

    /// <summary>
    /// Создаёт привычку.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="title">Название.</param>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="difficulty">Сложность.</param>
    /// <param name="daysOfWeek">Маска дней недели из 7 символов «0»/«1».</param>
    /// <returns>Результат проверки: список ошибок или успех.</returns>
    public ValidationResult Add(int userId, string title, int categoryId, Difficulty difficulty, string daysOfWeek)
    {
        ValidationResult validation = _validator.ValidateHabit(title, categoryId, difficulty, daysOfWeek);
        if (!validation.IsValid)
        {
            return validation;
        }

        using AppDbContext db = _dbFactory.CreateDbContext();
        if (!db.Users.Any(u => u.Id == userId))
        {
            return ValidationResult.Failure("Пользователь не найден.");
        }

        if (!db.Categories.Any(c => c.Id == categoryId))
        {
            return ValidationResult.Failure("Выбранная категория не найдена.");
        }

        db.Habits.Add(new Habit
        {
            UserId = userId,
            CategoryId = categoryId,
            Title = title.Trim(),
            Difficulty = difficulty,
            DaysOfWeek = daysOfWeek
        });
        db.SaveChanges();

        return ValidationResult.Success();
    }

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
    public ValidationResult Update(int userId, int habitId, string title, int categoryId, Difficulty difficulty, string daysOfWeek)
    {
        ValidationResult validation = _validator.ValidateHabit(title, categoryId, difficulty, daysOfWeek);
        if (!validation.IsValid)
        {
            return validation;
        }

        using AppDbContext db = _dbFactory.CreateDbContext();
        Habit? habit = db.Habits.FirstOrDefault(h => h.Id == habitId && h.UserId == userId);
        if (habit == null)
        {
            return ValidationResult.Failure("Привычка не найдена.");
        }

        if (!db.Categories.Any(c => c.Id == categoryId))
        {
            return ValidationResult.Failure("Выбранная категория не найдена.");
        }

        habit.Title = title.Trim();
        habit.CategoryId = categoryId;
        habit.Difficulty = difficulty;
        habit.DaysOfWeek = daysOfWeek;
        db.SaveChanges();

        return ValidationResult.Success();
    }

    /// <summary>
    /// Удаляет привычку вместе с её журналом.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <returns><c>true</c>, если привычка была удалена.</returns>
    public bool Delete(int userId, int habitId)
    {
        using AppDbContext db = _dbFactory.CreateDbContext();
        Habit? habit = db.Habits
            .Include(h => h.Logs)
            .FirstOrDefault(h => h.Id == habitId && h.UserId == userId);
        if (habit == null)
        {
            return false;
        }

        db.Habits.Remove(habit);
        db.SaveChanges();
        return true;
    }

    /// <summary>
    /// Отмечает привычку выполненной, начисляет опыт и пересчитывает уровень.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="date">День отметки.</param>
    /// <returns>Результат: полученный опыт и новый уровень либо ошибка.</returns>
    public CompletionResult MarkCompleted(int userId, int habitId, DateTime date)
    {
        DateTime day = date.Date;

        using AppDbContext db = _dbFactory.CreateDbContext();
        Habit? habit = db.Habits.FirstOrDefault(h => h.Id == habitId && h.UserId == userId);
        if (habit == null)
        {
            return CompletionResult.Fail("Привычка не найдена.");
        }

        if (!habit.IsScheduledFor(day))
        {
            return CompletionResult.Fail("Эта привычка не запланирована на выбранный день.");
        }

        User? user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return CompletionResult.Fail("Пользователь не найден.");
        }

        HabitLog log = GetOrCreateLog(db, habitId, day);
        int xp = _xpCalculator.CalculateXp(habit.Difficulty);

        try
        {
            log.Complete(xp);
        }
        catch (InvalidOperationException ex)
        {
            return CompletionResult.Fail(ex.Message);
        }

        int oldLevel = user.Level;
        user.AddXp(xp);
        user.Level = _xpCalculator.CalculateLevel(user.TotalXp);

        try
        {
            db.SaveChanges();
        }
        catch (DbUpdateException)
        {
            return CompletionResult.Fail("Не удалось сохранить отметку. Попробуйте ещё раз.");
        }

        return CompletionResult.Success(xp, user.TotalXp, user.Level, user.Level > oldLevel);
    }

    /// <summary>
    /// Отмечает привычку пропущенной.
    /// </summary>
    /// <param name="userId">Идентификатор владельца.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="date">День отметки.</param>
    /// <returns>Результат операции.</returns>
    public CompletionResult MarkSkipped(int userId, int habitId, DateTime date)
    {
        DateTime day = date.Date;

        using AppDbContext db = _dbFactory.CreateDbContext();
        Habit? habit = db.Habits.FirstOrDefault(h => h.Id == habitId && h.UserId == userId);
        if (habit == null)
        {
            return CompletionResult.Fail("Привычка не найдена.");
        }

        if (!habit.IsScheduledFor(day))
        {
            return CompletionResult.Fail("Эта привычка не запланирована на выбранный день.");
        }

        User? user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return CompletionResult.Fail("Пользователь не найден.");
        }

        HabitLog log = GetOrCreateLog(db, habitId, day);

        try
        {
            log.Skip();
        }
        catch (InvalidOperationException ex)
        {
            return CompletionResult.Fail(ex.Message);
        }

        try
        {
            db.SaveChanges();
        }
        catch (DbUpdateException)
        {
            return CompletionResult.Fail("Не удалось сохранить отметку. Попробуйте ещё раз.");
        }

        return CompletionResult.Success(0, user.TotalXp, user.Level, false);
    }

    /// <summary>
    /// Находит запись журнала за день или создаёт новую в состоянии «Активна».
    /// </summary>
    /// <param name="db">Контекст базы данных.</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="day">День (без времени).</param>
    /// <returns>Запись журнала.</returns>
    private static HabitLog GetOrCreateLog(AppDbContext db, int habitId, DateTime day)
    {
        HabitLog? log = db.HabitLogs.FirstOrDefault(l => l.HabitId == habitId && l.LogDate == day);
        if (log == null)
        {
            log = new HabitLog { HabitId = habitId, LogDate = day };
            db.HabitLogs.Add(log);
        }

        return log;
    }
}

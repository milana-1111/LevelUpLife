namespace LevelUpLife.Core.Models;

/// <summary>
/// Категория привычек (спорт, учёба, режим дня и т. д.).
/// </summary>
public class Category
{
    /// <summary>Идентификатор категории.</summary>
    public int Id { get; set; }

    /// <summary>Название категории. Уникально.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Привычки этой категории.</summary>
    public List<Habit> Habits { get; set; } = new();
}

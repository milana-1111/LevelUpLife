using LevelUpLife.Core.Enums;

namespace LevelUpLife.ViewModels;

/// <summary>
/// Русские названия перечислений для отображения в интерфейсе.
/// </summary>
public static class DisplayText
{
    public static string For(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => "Лёгкая",
            Difficulty.Medium => "Средняя",
            Difficulty.Hard => "Сложная",
            _ => difficulty.ToString()
        };
    }

    public static string For(HabitStatus status)
    {
        return status switch
        {
            HabitStatus.Active => "Активна",
            HabitStatus.Completed => "Выполнена",
            HabitStatus.Missed => "Пропущена",
            _ => status.ToString()
        };
    }
}

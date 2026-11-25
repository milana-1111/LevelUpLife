using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Services;

namespace LevelUpLife.ViewModels;

/// <summary>
/// Строка списка привычек на главном экране.
/// </summary>
public class HabitItemViewModel
{
    private readonly TodayHabit _habit;

    public HabitItemViewModel(TodayHabit habit)
    {
        _habit = habit;
    }

    public int HabitId => _habit.HabitId;
    public string Title => _habit.Title;

    public string CategoryName => _habit.CategoryName;

    public string DifficultyText => DisplayText.For(_habit.Difficulty);

    public string StatusText => _habit.IsScheduled ? DisplayText.For(_habit.Status) : "Не сегодня";

    public string XpText => _habit.Status == HabitStatus.Completed ? $"+{_habit.XpEarned}" : "—";

    public bool CanMark => _habit.IsScheduled && _habit.Status == HabitStatus.Active;
}

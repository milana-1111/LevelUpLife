using LevelUpLife.Infrastructure;

namespace LevelUpLife.ViewModels;

/// <summary>
/// День недели с флажком для окна редактирования привычки.
/// </summary>
public class DayOption : ObservableObject
{
    private bool _isSelected;

    public DayOption(string name, bool isSelected)
    {
        Name = name;
        _isSelected = isSelected;
    }

    public string Name { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

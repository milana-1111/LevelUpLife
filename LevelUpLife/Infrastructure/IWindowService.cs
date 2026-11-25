namespace LevelUpLife.Infrastructure;

/// <summary>
/// Открытие окон и диалогов. ViewModel работает только с этим интерфейсом
/// и ничего не знает о конкретных окнах WPF.
/// </summary>
public interface IWindowService
{
    void ShowLogin();

    void ShowMain();

    bool ShowHabitEdit(int? habitId);

    void ShowLeaderboard();

    void ShowAbout();

    bool Confirm(string message);

    void ShowMessage(string message);
}

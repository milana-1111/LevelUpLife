using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Services;
using LevelUpLife.Infrastructure;
using Serilog;

namespace LevelUpLife.ViewModels;

/// <summary>
/// Главное окно: профиль игрока, список привычек на сегодня и все действия меню.
/// </summary>
public class HabitListViewModel : ViewModelBase
{
    private readonly IHabitService _habits;
    private readonly IAuthService _auth;
    private readonly XpCalculator _xpCalculator;
    private readonly IUserContext _userContext;
    private readonly IWindowService _windows;

    private HabitItemViewModel? _selectedHabit;
    private bool _showAll;
    private string _displayName = string.Empty;
    private int _level = 1;
    private int _totalXp;
    private double _progressPercent;
    private string _progressText = string.Empty;
    private string _todayText = string.Empty;
    private string _statusMessage = string.Empty;

    public HabitListViewModel(
        IHabitService habits,
        IAuthService auth,
        XpCalculator xpCalculator,
        IUserContext userContext,
        IWindowService windows)
    {
        _habits = habits;
        _auth = auth;
        _xpCalculator = xpCalculator;
        _userContext = userContext;
        _windows = windows;

        CompleteCommand = new RelayCommand(Complete, CanMarkSelected);
        SkipCommand = new RelayCommand(Skip, CanMarkSelected);
        AddCommand = new RelayCommand(AddHabit);
        EditCommand = new RelayCommand(EditHabit, HasSelection);
        DeleteCommand = new RelayCommand(DeleteHabit, HasSelection);
        RefreshCommand = new RelayCommand(Load);
        OpenLeaderboardCommand = new RelayCommand(_windows.ShowLeaderboard);
        OpenAboutCommand = new RelayCommand(_windows.ShowAbout);
        LogoutCommand = new RelayCommand(Logout);
        ExitCommand = new RelayCommand(RequestClose);
    }
    public ObservableCollection<HabitItemViewModel> Habits { get; } = new();

    public HabitItemViewModel? SelectedHabit
    {
        get => _selectedHabit;
        set => SetProperty(ref _selectedHabit, value);
    }

    /// <summary>Показывать все привычки, а не только запланированные на сегодня.</summary>
    public bool ShowAll
    {
        get => _showAll;
        set
        {
            if (SetProperty(ref _showAll, value))
            {
                Load();
            }
        }
    }

    public string DisplayName
    {
        get => _displayName;
        private set => SetProperty(ref _displayName, value);
    }

    public int Level
    {
        get => _level;
        private set => SetProperty(ref _level, value);
    }

    public int TotalXp
    {
        get => _totalXp;
        private set => SetProperty(ref _totalXp, value);
    }

    public double ProgressPercent
    {
        get => _progressPercent;
        private set => SetProperty(ref _progressPercent, value);
    }

    public string ProgressText
    {
        get => _progressText;
        private set => SetProperty(ref _progressText, value);
    }

    public string TodayText
    {
        get => _todayText;
        private set => SetProperty(ref _todayText, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public ICommand CompleteCommand { get; }

    public ICommand SkipCommand { get; }

    public ICommand AddCommand { get; }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand OpenLeaderboardCommand { get; }

    public ICommand OpenAboutCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand ExitCommand { get; }
    public void Load()
    {
        try
        {
            ErrorMessage = string.Empty;
            RefreshUser();

            int selectedId = SelectedHabit?.HabitId ?? 0;
            DateTime today = DateTime.Today;
            var items = _habits.GetHabits(_userContext.CurrentUserId, today, !ShowAll);

            Habits.Clear();
            foreach (TodayHabit item in items)
            {
                Habits.Add(new HabitItemViewModel(item));
            }

            SelectedHabit = Habits.FirstOrDefault(h => h.HabitId == selectedId);
            TodayText = today.ToString("dddd, d MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка загрузки списка привычек");
            ErrorMessage = "Не удалось загрузить привычки.";
        }
    }

    /// <summary>
    /// Обновляет данные игрока и шкалу опыта.
    /// </summary>
    private void RefreshUser()
    {
        User? user = _auth.GetUser(_userContext.CurrentUserId);
        if (user == null)
        {
            return;
        }

        _userContext.Set(user);
        DisplayName = user.DisplayName;
        Level = user.Level;
        TotalXp = user.TotalXp;

        int levelStart = _xpCalculator.XpForLevel(user.Level);
        int levelEnd = _xpCalculator.XpForLevel(user.Level + 1);
        ProgressPercent = (user.TotalXp - levelStart) * 100.0 / (levelEnd - levelStart);
        ProgressText = $"{user.TotalXp} / {levelEnd} XP";
    }
    private bool CanMarkSelected()
    {
        return SelectedHabit is { CanMark: true };
    }
    private bool HasSelection()
    {
        return SelectedHabit != null;
    }
    private void Complete()
    {
        if (SelectedHabit == null)
        {
            return;
        }

        try
        {
            CompletionResult result = _habits.MarkCompleted(_userContext.CurrentUserId, SelectedHabit.HabitId, DateTime.Today);
            if (!result.IsSuccess)
            {
                StatusMessage = string.Empty;
                ErrorMessage = result.ErrorMessage;
                return;
            }

            StatusMessage = result.LeveledUp
                ? $"Выполнено! +{result.XpEarned} XP. Новый уровень: {result.Level}!"
                : $"Выполнено! +{result.XpEarned} XP.";
            Load();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при отметке привычки");
            ErrorMessage = "Не удалось отметить привычку.";
        }
    }
    private void Skip()
    {
        if (SelectedHabit == null)
        {
            return;
        }

        try
        {
            CompletionResult result = _habits.MarkSkipped(_userContext.CurrentUserId, SelectedHabit.HabitId, DateTime.Today);
            if (!result.IsSuccess)
            {
                StatusMessage = string.Empty;
                ErrorMessage = result.ErrorMessage;
                return;
            }

            StatusMessage = "Привычка отмечена как пропущенная.";
            Load();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при пропуске привычки");
            ErrorMessage = "Не удалось отметить привычку.";
        }
    }
    private void AddHabit()
    {
        if (_windows.ShowHabitEdit(null))
        {
            StatusMessage = "Привычка добавлена.";
            Load();
        }
    }
    private void EditHabit()
    {
        if (SelectedHabit != null && _windows.ShowHabitEdit(SelectedHabit.HabitId))
        {
            StatusMessage = "Привычка изменена.";
            Load();
        }
    }
    private void DeleteHabit()
    {
        if (SelectedHabit == null)
        {
            return;
        }

        if (!_windows.Confirm($"Удалить привычку «{SelectedHabit.Title}» вместе с историей выполнения?"))
        {
            return;
        }

        try
        {
            _habits.Delete(_userContext.CurrentUserId, SelectedHabit.HabitId);
            StatusMessage = "Привычка удалена.";
            Load();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при удалении привычки");
            ErrorMessage = "Не удалось удалить привычку.";
        }
    }
    private void Logout()
    {
        try
        {
            _auth.Logout();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при выходе из аккаунта");
        }

        _userContext.Clear();
        _windows.ShowLogin();
        RequestClose();
    }
}

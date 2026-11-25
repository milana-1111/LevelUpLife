using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Services;
using LevelUpLife.Core.Validation;
using LevelUpLife.Infrastructure;
using Serilog;

namespace LevelUpLife.ViewModels;

/// <summary>
/// Окно создания и редактирования привычки.
/// </summary>
public class HabitEditViewModel : ViewModelBase
{
    private static readonly string[] DayNames = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };

    private readonly IHabitService _habits;
    private readonly IUserContext _userContext;

    private int? _habitId;
    private string _title = string.Empty;
    private Category? _selectedCategory;
    private DifficultyOption? _selectedDifficulty;

    public HabitEditViewModel(IHabitService habits, IUserContext userContext)
    {
        _habits = habits;
        _userContext = userContext;

        DifficultyOptions = Enum.GetValues<Difficulty>()
            .Select(d => new DifficultyOption(d, DisplayText.For(d)))
            .ToList();
        Days = new ObservableCollection<DayOption>(DayNames.Select(n => new DayOption(n, true)));

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(RequestClose);
    }

    public ObservableCollection<Category> Categories { get; } = new();

    public IReadOnlyList<DifficultyOption> DifficultyOptions { get; }

    public ObservableCollection<DayOption> Days { get; }

    public string WindowTitle => _habitId == null ? "Новая привычка" : "Изменение привычки";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public DifficultyOption? SelectedDifficulty
    {
        get => _selectedDifficulty;
        set => SetProperty(ref _selectedDifficulty, value);
    }

    public bool Saved { get; private set; }

    public ICommand SaveCommand { get; }

    public ICommand CancelCommand { get; }

    public void Load(int? habitId)
    {
        _habitId = habitId;
        Saved = false;
        ErrorMessage = string.Empty;

        try
        {
            Categories.Clear();
            foreach (Category category in _habits.GetCategories())
            {
                Categories.Add(category);
            }

            Title = string.Empty;
            SelectedCategory = Categories.FirstOrDefault();
            SelectedDifficulty = DifficultyOptions[0];
            foreach (DayOption day in Days)
            {
                day.IsSelected = true;
            }

            if (habitId != null)
            {
                Habit? habit = _habits.GetHabit(_userContext.CurrentUserId, habitId.Value);
                if (habit == null)
                {
                    ErrorMessage = "Привычка не найдена.";
                }
                else
                {
                    Title = habit.Title;
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == habit.CategoryId);
                    SelectedDifficulty = DifficultyOptions.First(o => o.Value == habit.Difficulty);
                    for (int i = 0; i < Days.Count; i++)
                    {
                        Days[i].IsSelected = habit.DaysOfWeek.Length == 7 && habit.DaysOfWeek[i] == '1';
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка загрузки данных привычки");
            ErrorMessage = "Не удалось загрузить данные.";
        }

        OnPropertyChanged(nameof(WindowTitle));
    }

    private void Save()
    {
        ErrorMessage = string.Empty;

        string mask = new string(Days.Select(d => d.IsSelected ? '1' : '0').ToArray());
        int categoryId = SelectedCategory?.Id ?? 0;
        Difficulty difficulty = SelectedDifficulty?.Value ?? 0;

        try
        {
            ValidationResult result = _habitId == null
                ? _habits.Add(_userContext.CurrentUserId, Title, categoryId, difficulty, mask)
                : _habits.Update(_userContext.CurrentUserId, _habitId.Value, Title, categoryId, difficulty, mask);

            if (!result.IsValid)
            {
                ErrorMessage = result.ErrorsText;
                return;
            }

            Saved = true;
            RequestClose();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка сохранения привычки");
            ErrorMessage = "Не удалось сохранить привычку. Попробуйте ещё раз.";
        }
    }
}

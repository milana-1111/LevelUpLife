using System.Windows;
using LevelUpLife.ViewModels;

namespace LevelUpLife.Views;

/// <summary>
/// Окно создания и редактирования привычки. Открывается как диалог.
/// </summary>
public partial class HabitEditWindow : Window
{
    /// <summary>
    /// Создаёт окно и подключает ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel окна привычки.</param>
    public HabitEditWindow(HabitEditViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += (_, _) => DialogResult = viewModel.Saved;
    }
}

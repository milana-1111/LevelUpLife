using System.Windows;
using LevelUpLife.ViewModels;

namespace LevelUpLife.Views;

/// <summary>
/// Окно таблицы лидеров. Открывается как диалог.
/// </summary>
public partial class LeaderboardWindow : Window
{
    /// <summary>
    /// Создаёт окно и подключает ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel таблицы лидеров.</param>
    public LeaderboardWindow(LeaderboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += (_, _) => Close();
    }
}

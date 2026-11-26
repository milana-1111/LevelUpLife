using System.Windows;
using LevelUpLife.ViewModels;

namespace LevelUpLife.Views;

/// <summary>
/// Главное окно приложения: профиль, список привычек и меню.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Создаёт окно, подключает ViewModel и загружает данные после открытия.
    /// </summary>
    /// <param name="viewModel">ViewModel главного окна.</param>
    public MainWindow(HabitListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += (_, _) => Close();
        Loaded += (_, _) => viewModel.Load();
    }
}

using System;
using System.Windows;
using LevelUpLife.ViewModels;
using LevelUpLife.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LevelUpLife.Infrastructure;

/// <summary>
/// Открывает окна приложения. Окна создаёт контейнер зависимостей,
/// поэтому ViewModel подставляются автоматически.
/// </summary>
public class WindowService : IWindowService
{
    private const string AppTitle = "Level Up Life";

    private readonly IServiceProvider _provider;
    private Window? _mainWindow;

    /// <summary>
    /// Создаёт сервис окон.
    /// </summary>
    /// <param name="provider">Контейнер зависимостей.</param>
    public WindowService(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Открывает окно входа и регистрации.
    /// </summary>
    public void ShowLogin()
    {
        _provider.GetRequiredService<LoginWindow>().Show();
    }

    /// <summary>
    /// Открывает главное окно.
    /// </summary>
    public void ShowMain()
    {
        var window = _provider.GetRequiredService<MainWindow>();
        _mainWindow = window;
        window.Show();
    }

    /// <summary>
    /// Открывает окно создания или редактирования привычки.
    /// </summary>
    /// <param name="habitId">Идентификатор привычки или <c>null</c>, если создаётся новая.</param>
    /// <returns><c>true</c>, если привычка была сохранена.</returns>
    public bool ShowHabitEdit(int? habitId)
    {
        var window = _provider.GetRequiredService<HabitEditWindow>();
        ((HabitEditViewModel)window.DataContext).Load(habitId);
        window.Owner = _mainWindow;
        return window.ShowDialog() == true;
    }

    /// <summary>
    /// Открывает окно таблицы лидеров.
    /// </summary>
    public void ShowLeaderboard()
    {
        var window = _provider.GetRequiredService<LeaderboardWindow>();
        ((LeaderboardViewModel)window.DataContext).Load();
        window.Owner = _mainWindow;
        window.ShowDialog();
    }

    /// <summary>
    /// Открывает окно «О программе».
    /// </summary>
    public void ShowAbout()
    {
        var window = _provider.GetRequiredService<AboutWindow>();
        window.Owner = _mainWindow;
        window.ShowDialog();
    }

    /// <summary>
    /// Показывает вопрос с кнопками «Да» и «Нет».
    /// </summary>
    /// <param name="message">Текст вопроса.</param>
    /// <returns><c>true</c>, если пользователь выбрал «Да».</returns>
    public bool Confirm(string message)
    {
        MessageBoxResult result = _mainWindow != null
            ? MessageBox.Show(_mainWindow, message, AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question)
            : MessageBox.Show(message, AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    /// <summary>
    /// Показывает информационное сообщение.
    /// </summary>
    /// <param name="message">Текст сообщения.</param>
    public void ShowMessage(string message)
    {
        if (_mainWindow != null)
        {
            MessageBox.Show(_mainWindow, message, AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(message, AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

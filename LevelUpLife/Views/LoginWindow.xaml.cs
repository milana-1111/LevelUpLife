using System.Windows;
using System.Windows.Controls;
using LevelUpLife.ViewModels;

namespace LevelUpLife.Views;

/// <summary>
/// Окно входа и регистрации.
/// </summary>
public partial class LoginWindow : Window
{
    /// <summary>
    /// Создаёт окно и подключает к нему ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel окна входа.</param>
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += (_, _) => Close();
    }

    /// <summary>
    /// Передаёт введённый пароль во ViewModel (PasswordBox не поддерживает привязку).
    /// </summary>
    /// <param name="sender">Поле ввода пароля.</param>
    /// <param name="e">Данные события.</param>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }
}

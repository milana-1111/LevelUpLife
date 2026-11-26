using System.Windows;
using System.Windows.Input;

namespace LevelUpLife.Views;

/// <summary>
/// Окно «О программе»: тема проекта и исполнитель. Открывается из меню «Справка».
/// </summary>
public partial class AboutWindow : Window
{
    /// <summary>
    /// Создаёт окно.
    /// </summary>
    public AboutWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Закрывает окно по кнопке «ОК».
    /// </summary>
    /// <param name="sender">Кнопка «ОК».</param>
    /// <param name="e">Данные события.</param>
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Закрывает окно по клавише Esc.
    /// </summary>
    /// <param name="sender">Окно.</param>
    /// <param name="e">Данные нажатия клавиши.</param>
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}

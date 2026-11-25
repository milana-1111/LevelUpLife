using System;
using LevelUpLife.Infrastructure;

namespace LevelUpLife.ViewModels;

/// <summary>
/// Базовый класс всех ViewModel: уведомления об изменении свойств,
/// сообщение об ошибке и запрос на закрытие окна.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    private string _errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => _errorMessage;
        protected set => SetProperty(ref _errorMessage, value);
    }
    public event EventHandler? CloseRequested;
    protected void RequestClose()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}

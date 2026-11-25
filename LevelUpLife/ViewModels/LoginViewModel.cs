using System;
using System.Windows.Input;
using LevelUpLife.Core.Services;
using LevelUpLife.Infrastructure;
using Serilog;

namespace LevelUpLife.ViewModels;

/// <summary>
/// Окно входа и регистрации. Один и тот же экран работает в двух режимах.
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly IUserContext _userContext;
    private readonly IWindowService _windows;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _displayName = string.Empty;
    private bool _isRegisterMode;
    public LoginViewModel(IAuthService auth, IUserContext userContext, IWindowService windows)
    {
        _auth = auth;
        _userContext = userContext;
        _windows = windows;

        SubmitCommand = new RelayCommand(Submit);
        ToggleModeCommand = new RelayCommand(ToggleMode);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    public bool IsRegisterMode
    {
        get => _isRegisterMode;
        private set
        {
            if (SetProperty(ref _isRegisterMode, value))
            {
                OnPropertyChanged(nameof(SubmitText));
                OnPropertyChanged(nameof(ToggleText));
                OnPropertyChanged(nameof(HeaderText));
            }
        }
    }

    public string HeaderText => IsRegisterMode ? "Регистрация" : "Вход";
    public string SubmitText => IsRegisterMode ? "Зарегистрироваться" : "Войти";
    public string ToggleText => IsRegisterMode ? "Уже есть аккаунт? Войти" : "Нет аккаунта? Зарегистрироваться";
    public ICommand SubmitCommand { get; }
    public ICommand ToggleModeCommand { get; }

    /// <summary>
    /// Выполняет вход или регистрацию. При успехе открывает главное окно и закрывает окно входа.
    /// </summary>
    private void Submit()
    {
        ErrorMessage = string.Empty;

        try
        {
            AuthResult result = IsRegisterMode
                ? _auth.Register(Username, Password, DisplayName)
                : _auth.Login(Username, Password);

            if (!result.IsSuccess)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            _userContext.Set(result.User!);
            Password = string.Empty;
            _windows.ShowMain();
            RequestClose();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при входе или регистрации");
            ErrorMessage = "Не удалось выполнить операцию. Попробуйте ещё раз.";
        }
    }

    /// <summary>
    /// Переключает режим окна между входом и регистрацией.
    /// </summary>
    private void ToggleMode()
    {
        ErrorMessage = string.Empty;
        IsRegisterMode = !IsRegisterMode;
    }
}

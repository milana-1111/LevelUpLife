using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using LevelUpLife.Core.Data;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Security;
using LevelUpLife.Core.Services;
using LevelUpLife.Core.Validation;
using LevelUpLife.Infrastructure;
using LevelUpLife.ViewModels;
using LevelUpLife.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LevelUpLife;

/// <summary>
/// Точка входа приложения: настройка внедрения зависимостей, логов, базы данных
/// и выбор первого окна (вход или главное окно, если сессия сохранилась).
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _provider;

    /// <summary>
    /// Запускает приложение: готовит папку данных, логи, сервисы и базу, затем открывает первое окно.
    /// </summary>
    /// <param name="e">Параметры запуска.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        string dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LevelUpLife");

        try
        {
            Directory.CreateDirectory(dataDirectory);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(dataDirectory, "logs", "app-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            DispatcherUnhandledException += OnDispatcherUnhandledException;

            _provider = ConfigureServices(dataDirectory);
            InitializeDatabase(_provider);
            StartFirstWindow(_provider);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Не удалось запустить приложение");
            MessageBox.Show(
                "Не удалось запустить приложение. Подробности записаны в файл журнала.",
                "Level Up Life",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    /// <summary>
    /// Завершает работу: сбрасывает логи и освобождает сервисы.
    /// </summary>
    /// <param name="e">Параметры завершения.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        _provider?.Dispose();
        base.OnExit(e);
    }

    /// <summary>
    /// Регистрирует все сервисы, ViewModel и окна в контейнере зависимостей.
    /// </summary>
    /// <param name="dataDirectory">Папка для базы данных, токена сессии и логов.</param>
    /// <returns>Готовый контейнер зависимостей.</returns>
    private static ServiceProvider ConfigureServices(string dataDirectory)
    {
        string databasePath = Path.Combine(dataDirectory, "levelup.db");
        string sessionPath = Path.Combine(dataDirectory, "session.token");

        var services = new ServiceCollection();

        services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite($"Data Source={databasePath}"));

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<InputValidator>();
        services.AddSingleton<XpCalculator>();
        services.AddSingleton<ISessionStore>(_ => new FileSessionStore(sessionPath));
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IHabitService, HabitService>();
        services.AddSingleton<ILeaderboardService, LeaderboardService>();
        services.AddSingleton<IUserContext, UserContext>();
        services.AddSingleton<IWindowService, WindowService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<HabitListViewModel>();
        services.AddTransient<HabitEditViewModel>();
        services.AddTransient<LeaderboardViewModel>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<HabitEditWindow>();
        services.AddTransient<LeaderboardWindow>();
        services.AddTransient<AboutWindow>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Создаёт файл базы данных и таблицы, если их ещё нет.
    /// </summary>
    /// <param name="provider">Контейнер зависимостей.</param>
    private static void InitializeDatabase(IServiceProvider provider)
    {
        var factory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using AppDbContext db = factory.CreateDbContext();
        db.Database.EnsureCreated();
    }

    /// <summary>
    /// Пытается восстановить сессию. Если получилось, открывает главное окно, иначе окно входа.
    /// </summary>
    /// <param name="provider">Контейнер зависимостей.</param>
    private static void StartFirstWindow(IServiceProvider provider)
    {
        var auth = provider.GetRequiredService<IAuthService>();
        var userContext = provider.GetRequiredService<IUserContext>();
        var windows = provider.GetRequiredService<IWindowService>();

        AuthResult session = auth.RestoreSession();
        if (session.IsSuccess)
        {
            userContext.Set(session.User!);
            windows.ShowMain();
        }
        else
        {
            windows.ShowLogin();
        }
    }

    /// <summary>
    /// Перехватывает необработанные ошибки интерфейса, чтобы приложение не закрывалось.
    /// </summary>
    /// <param name="sender">Источник события.</param>
    /// <param name="e">Данные об ошибке.</param>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Необработанная ошибка интерфейса");
        MessageBox.Show(
            "Произошла непредвиденная ошибка. Подробности записаны в файл журнала.",
            "Level Up Life",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
        e.Handled = true;
    }
}

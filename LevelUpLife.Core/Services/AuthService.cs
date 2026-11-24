using System.Security.Cryptography;
using LevelUpLife.Core.Data;
using LevelUpLife.Core.Models;
using LevelUpLife.Core.Security;
using LevelUpLife.Core.Validation;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLife.Core.Services;

/// <summary>
/// Регистрация, вход и сессии пользователей.
/// Логин хранится в нижнем регистре, поэтому «Milana» и «milana» — один и тот же пользователь.
/// </summary>
public class AuthService : IAuthService
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(30);

    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly IPasswordHasher _hasher;
    private readonly InputValidator _validator;
    private readonly ISessionStore _sessionStore;

    /// <summary>
    /// Создаёт сервис авторизации.
    /// </summary>
    /// <param name="dbFactory">Фабрика контекстов базы данных.</param>
    /// <param name="hasher">Сервис хеширования паролей.</param>
    /// <param name="validator">Проверка вводимых данных.</param>
    /// <param name="sessionStore">Хранилище токена сессии.</param>
    public AuthService(
        IDbContextFactory<AppDbContext> dbFactory,
        IPasswordHasher hasher,
        InputValidator validator,
        ISessionStore sessionStore)
    {
        _dbFactory = dbFactory;
        _hasher = hasher;
        _validator = validator;
        _sessionStore = sessionStore;
    }

    /// <summary>
    /// Регистрирует нового пользователя и сразу входит в систему.
    /// </summary>
    /// <param name="username">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="displayName">Имя в таблице лидеров.</param>
    /// <returns>Результат операции.</returns>
    public AuthResult Register(string username, string password, string displayName)
    {
        ValidationResult validation = _validator.ValidateRegistration(username, password, displayName);
        if (!validation.IsValid)
        {
            return AuthResult.Fail(validation.ErrorsText);
        }

        string login = NormalizeLogin(username);

        using AppDbContext db = _dbFactory.CreateDbContext();
        if (db.Users.Any(u => u.Username == login))
        {
            return AuthResult.Fail("Пользователь с таким логином уже существует.");
        }

        var user = new User
        {
            Username = login,
            PasswordHash = _hasher.Hash(password),
            DisplayName = displayName.Trim()
        };

        db.Users.Add(user);
        try
        {
            db.SaveChanges();
        }
        catch (DbUpdateException)
        {
            return AuthResult.Fail("Пользователь с таким логином уже существует.");
        }

        StartSession(db, user);
        return AuthResult.Success(user);
    }

    /// <summary>
    /// Выполняет вход по логину и паролю.
    /// </summary>
    /// <param name="username">Логин.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Результат операции.</returns>
    public AuthResult Login(string username, string password)
    {
        ValidationResult validation = _validator.ValidateLogin(username, password);
        if (!validation.IsValid)
        {
            return AuthResult.Fail(validation.ErrorsText);
        }

        string login = NormalizeLogin(username);

        using AppDbContext db = _dbFactory.CreateDbContext();
        User? user = db.Users.FirstOrDefault(u => u.Username == login);
        if (user == null || !_hasher.Verify(password, user.PasswordHash))
        {
            return AuthResult.Fail("Неверный логин или пароль.");
        }

        StartSession(db, user);
        return AuthResult.Success(user);
    }

    /// <summary>
    /// Завершает текущую сессию и удаляет сохранённый токен.
    /// </summary>
    public void Logout()
    {
        string? token = _sessionStore.Load();
        if (token != null)
        {
            using AppDbContext db = _dbFactory.CreateDbContext();
            Session? session = db.Sessions.FirstOrDefault(s => s.Token == token);
            if (session != null)
            {
                db.Sessions.Remove(session);
                db.SaveChanges();
            }
        }

        _sessionStore.Clear();
    }

    /// <summary>
    /// Восстанавливает вход по сохранённому токену (после перезапуска приложения).
    /// </summary>
    /// <returns>Результат операции.</returns>
    public AuthResult RestoreSession()
    {
        string? token = _sessionStore.Load();
        if (token == null)
        {
            return AuthResult.Fail("Сессия не найдена.");
        }

        using AppDbContext db = _dbFactory.CreateDbContext();
        Session? session = db.Sessions.Include(s => s.User).FirstOrDefault(s => s.Token == token);
        if (session == null || session.User == null || session.IsExpired())
        {
            _sessionStore.Clear();
            return AuthResult.Fail("Сессия недействительна. Войдите заново.");
        }

        return AuthResult.Success(session.User);
    }

    /// <summary>
    /// Загружает актуальные данные пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Пользователь или <c>null</c>, если не найден.</returns>
    public User? GetUser(int userId)
    {
        using AppDbContext db = _dbFactory.CreateDbContext();
        return db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
    }

    /// <summary>
    /// Приводит логин к единому виду: без пробелов по краям, в нижнем регистре.
    /// </summary>
    /// <param name="username">Введённый логин.</param>
    /// <returns>Нормализованный логин.</returns>
    private static string NormalizeLogin(string username)
    {
        return username.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Создаёт новую сессию пользователя, удаляет его просроченные сессии и запоминает токен.
    /// </summary>
    /// <param name="db">Контекст базы данных.</param>
    /// <param name="user">Пользователь, для которого создаётся сессия.</param>
    private void StartSession(AppDbContext db, User user)
    {
        DateTime now = DateTime.UtcNow;
        List<Session> expired = db.Sessions.Where(s => s.UserId == user.Id && s.ExpiresAt <= now).ToList();
        db.Sessions.RemoveRange(expired);

        string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        db.Sessions.Add(new Session
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = now.Add(SessionLifetime)
        });
        db.SaveChanges();

        _sessionStore.Save(token);
    }
}

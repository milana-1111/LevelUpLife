using LevelUpLife.Core.Security;
using LevelUpLife.Core.Services;
using LevelUpLife.Core.Validation;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Тесты регистрации, входа и сессий.
/// </summary>
public class AuthServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly InMemorySessionStore _store = new();
    private readonly AuthService _auth;

    /// <summary>
    /// Создаёт сервис авторизации на чистой базе.
    /// </summary>
    public AuthServiceTests()
    {
        _auth = new AuthService(_database, new PasswordHasher(), new InputValidator(), _store);
    }

    /// <summary>
    /// Освобождает базу данных.
    /// </summary>
    public void Dispose()
    {
        _database.Dispose();
    }

    [Fact]
    public void Register_ValidData_CreatesUserAndSession()
    {
        var result = _auth.Register("milana", "secret1", "Милана");

        Assert.True(result.IsSuccess);
        Assert.Equal("milana", result.User!.Username);
        Assert.NotEqual("secret1", result.User.PasswordHash);
        Assert.NotNull(_store.Load());
    }

    [Fact]
    public void Register_StartsWithLevelOneAndZeroXp()
    {
        var user = _auth.Register("milana", "secret1", "Милана").User!;

        Assert.Equal(1, user.Level);
        Assert.Equal(0, user.TotalXp);
    }

    [Fact]
    public void Register_DuplicateLoginIgnoringCase_Fails()
    {
        _auth.Register("milana", "secret1", "Милана");

        var result = _auth.Register("MILANA", "secret2", "Другая");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Register_InvalidData_Fails()
    {
        var result = _auth.Register("a", "1", "");

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.ErrorMessage);
    }

    [Fact]
    public void Login_CorrectPassword_Succeeds()
    {
        _auth.Register("milana", "secret1", "Милана");

        var result = _auth.Login("Milana", "secret1");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Login_WrongPassword_Fails()
    {
        _auth.Register("milana", "secret1", "Милана");

        var result = _auth.Login("milana", "wrong-password");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Login_UnknownUser_Fails()
    {
        Assert.False(_auth.Login("nobody", "secret1").IsSuccess);
    }

    [Fact]
    public void RestoreSession_AfterLogin_ReturnsSameUser()
    {
        var user = _auth.Register("milana", "secret1", "Милана").User!;

        var restored = _auth.RestoreSession();

        Assert.True(restored.IsSuccess);
        Assert.Equal(user.Id, restored.User!.Id);
    }

    [Fact]
    public void RestoreSession_WithoutToken_Fails()
    {
        Assert.False(_auth.RestoreSession().IsSuccess);
    }

    [Fact]
    public void Logout_RemovesSession()
    {
        _auth.Register("milana", "secret1", "Милана");

        _auth.Logout();

        Assert.Null(_store.Load());
        Assert.False(_auth.RestoreSession().IsSuccess);
    }

    [Fact]
    public void RestoreSession_ExpiredSession_Fails()
    {
        _auth.Register("milana", "secret1", "Милана");
        using (var db = _database.CreateDbContext())
        {
            var session = db.Sessions.Single();
            session.ExpiresAt = DateTime.UtcNow.AddDays(-1);
            db.SaveChanges();
        }

        Assert.False(_auth.RestoreSession().IsSuccess);
        Assert.Null(_store.Load());
    }

    [Fact]
    public void GetUser_ReturnsUserById()
    {
        var user = _auth.Register("milana", "secret1", "Милана").User!;

        Assert.Equal("Милана", _auth.GetUser(user.Id)!.DisplayName);
        Assert.Null(_auth.GetUser(9999));
    }
}

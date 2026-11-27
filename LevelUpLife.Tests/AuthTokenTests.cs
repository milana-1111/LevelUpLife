using LevelUpLife.Core.Security;
using LevelUpLife.Core.Services;
using LevelUpLife.Core.Validation;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Тесты серверной проверки токенов сессии.
/// </summary>
public class AuthTokenTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly AuthService _auth;

    /// <summary>
    /// Создаёт сервис так же, как на сервере: хранилище токена ничего не запоминает.
    /// </summary>
    public AuthTokenTests()
    {
        _auth = new AuthService(_database, new PasswordHasher(), new InputValidator(), new NullSessionStore());
    }

    /// <summary>
    /// Освобождает базу данных.
    /// </summary>
    public void Dispose()
    {
        _database.Dispose();
    }

    [Fact]
    public void Register_ReturnsToken()
    {
        var result = _auth.Register("milana", "secret1", "Милана");

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public void Login_ReturnsNewTokenEachTime()
    {
        _auth.Register("milana", "secret1", "Милана");

        string first = _auth.Login("milana", "secret1").Token!;
        string second = _auth.Login("milana", "secret1").Token!;

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Authenticate_ValidToken_ReturnsUser()
    {
        var registered = _auth.Register("milana", "secret1", "Милана");

        var result = _auth.Authenticate(registered.Token!);

        Assert.True(result.IsSuccess);
        Assert.Equal(registered.User!.Id, result.User!.Id);
    }

    [Fact]
    public void Authenticate_UnknownToken_Fails()
    {
        Assert.False(_auth.Authenticate("no-such-token").IsSuccess);
    }

    [Fact]
    public void Revoke_MakesTokenInvalid()
    {
        string token = _auth.Register("milana", "secret1", "Милана").Token!;

        _auth.Revoke(token);

        Assert.False(_auth.Authenticate(token).IsSuccess);
    }

    [Fact]
    public void Authenticate_ExpiredToken_Fails()
    {
        string token = _auth.Register("milana", "secret1", "Милана").Token!;
        using (var db = _database.CreateDbContext())
        {
            var session = db.Sessions.Single();
            session.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);
            db.SaveChanges();
        }

        Assert.False(_auth.Authenticate(token).IsSuccess);
    }

    [Fact]
    public void OneUsersTokenDoesNotGiveAnotherUsersData()
    {
        string tokenA = _auth.Register("anna", "secret1", "Анна").Token!;
        _auth.Register("boris", "secret1", "Борис");

        var result = _auth.Authenticate(tokenA);

        Assert.Equal("anna", result.User!.Username);
    }
}

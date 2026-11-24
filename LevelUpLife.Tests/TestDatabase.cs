using LevelUpLife.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LevelUpLife.Tests;

/// <summary>
/// Чистая база SQLite в памяти для тестов. Работает как фабрика контекстов,
/// поэтому её можно передавать сервисам вместо настоящей фабрики.
/// </summary>
public sealed class TestDatabase : IDbContextFactory<AppDbContext>, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    /// <summary>
    /// Открывает соединение, включает внешние ключи и создаёт таблицы.
    /// </summary>
    public TestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using (SqliteCommand command = _connection.CreateCommand())
        {
            command.CommandText = "PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();
        }

        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        using AppDbContext db = CreateDbContext();
        db.Database.EnsureCreated();
    }

    /// <summary>
    /// Создаёт новый контекст, подключённый к базе в памяти.
    /// </summary>
    /// <returns>Новый контекст базы данных.</returns>
    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }

    /// <summary>
    /// Закрывает соединение, база в памяти при этом удаляется.
    /// </summary>
    public void Dispose()
    {
        _connection.Dispose();
    }
}

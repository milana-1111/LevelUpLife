using LevelUpLife.Core.Services;

namespace LevelUpLife.Tests;

/// <summary>
/// Хранилище токена в памяти для тестов (файлы не нужны).
/// </summary>
public class InMemorySessionStore : ISessionStore
{
    private string? _token;

    /// <summary>
    /// Возвращает сохранённый токен.
    /// </summary>
    /// <returns>Токен или <c>null</c>.</returns>
    public string? Load() => _token;

    /// <summary>
    /// Запоминает токен.
    /// </summary>
    /// <param name="token">Токен сессии.</param>
    public void Save(string token) => _token = token;

    /// <summary>
    /// Забывает токен.
    /// </summary>
    public void Clear() => _token = null;
}

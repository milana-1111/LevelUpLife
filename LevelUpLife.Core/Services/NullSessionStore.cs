namespace LevelUpLife.Core.Services;

/// <summary>
/// Хранилище токена, которое ничего не хранит. Нужно серверу:
/// токен принадлежит клиенту, а не серверу, поэтому сервер его не запоминает.
/// </summary>
public class NullSessionStore : ISessionStore
{
    /// <summary>
    /// Токена нет никогда.
    /// </summary>
    /// <returns>Всегда <c>null</c>.</returns>
    public string? Load() => null;

    /// <summary>
    /// Ничего не сохраняет.
    /// </summary>
    /// <param name="token">Токен сессии (игнорируется).</param>
    public void Save(string token)
    {
    }

    /// <summary>
    /// Ничего не удаляет.
    /// </summary>
    public void Clear()
    {
    }
}

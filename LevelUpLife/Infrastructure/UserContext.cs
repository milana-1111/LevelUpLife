using LevelUpLife.Core.Models;

namespace LevelUpLife.Infrastructure;

/// <summary>
/// Простая реализация хранилища текущего пользователя.
/// </summary>
public class UserContext : IUserContext
{
    public User? CurrentUser { get; private set; }

    public int CurrentUserId => CurrentUser?.Id ?? 0;

    public void Set(User user)
    {
        CurrentUser = user;
    }

    public void Clear()
    {
        CurrentUser = null;
    }
}

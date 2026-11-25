using LevelUpLife.Core.Models;

namespace LevelUpLife.Infrastructure;

/// <summary>
/// Хранит пользователя, который сейчас вошёл в приложение.
/// </summary>
public interface IUserContext
{
    User? CurrentUser { get; }

    int CurrentUserId { get; }

    void Set(User user);

    void Clear();
}

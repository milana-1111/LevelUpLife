namespace LevelUpLife.Core.Models;

//Сессия авторизации
public class Session
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public User? User { get; set; }

    //Проверяет, истёк ли срок действия сессии
    public bool IsExpired()
    {
        return ExpiresAt <= DateTime.UtcNow;
    }
}

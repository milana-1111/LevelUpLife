namespace LevelUpLife.Core.Models;

//Пользователь приложения
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TotalXp { get; set; }
    public int Level { get; set; } = 1;
    public List<Session> Sessions { get; set; } = new();
    public List<Habit> Habits { get; set; } = new();

    //Добавляет пользователю опыт
    public void AddXp(int xp)
    {
        if (xp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(xp), "Количество XP не может быть отрицательным.");
        }

        TotalXp += xp;
    }
}

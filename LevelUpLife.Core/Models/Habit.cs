using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.Models;

//Привычка пользователя
public class Habit
{
    //Идентификаторы привычки
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    //Название привычки
    public string Title { get; set; } = string.Empty;

    //Сложность привычки
    public Difficulty Difficulty { get; set; } = Difficulty.Easy;

    //Дни недели, в которые нужно выполнять привычку: 7 символов «0» или «1»,
    //по порядку с понедельника по воскресенье. Например, «1111100» — по будням
    public string DaysOfWeek { get; set; } = "1111111";
    public User? User { get; set; }

    public Category? Category { get; set; }

    public List<HabitLog> Logs { get; set; } = new();

    //Проверяет, запланирована ли привычка на указанный день
      public bool IsScheduledFor(DateTime date)
    {
        if (DaysOfWeek.Length != 7)
        {
            return false;
        }

        // DayOfWeek считает с воскресенья (0), а в маске понедельник идёт первым.
        int index = ((int)date.DayOfWeek + 6) % 7;
        return DaysOfWeek[index] == '1';
    }
}

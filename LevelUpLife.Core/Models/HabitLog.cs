using LevelUpLife.Core.Enums;
using LevelUpLife.Core.States;

namespace LevelUpLife.Core.Models;

//Запись журнала: состояние одной привычки за один день
//Пара «привычка + дата» уникальна
public class HabitLog
{
    public int Id { get; set; }

    public int HabitId { get; set; }

    public DateTime LogDate { get; set; }

    public HabitStatus Status { get; set; } = HabitStatus.Active;

    public int XpEarned { get; set; }

    public Habit? Habit { get; set; }

    //Отмечает привычку выполненной и запоминает начисленный опыт

    public IHabitState GetState()
    {
        return HabitStateFactory.Create(Status);
    }

    public void Complete(int xp)
    {
        if (xp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(xp), "Количество XP не может быть отрицательным.");
        }

        GetState().Complete(this, xp);
    }

    //Отмечает привычку пропущенной. Опыт за такой день не начисляется
    public void Skip()
    {
        GetState().Skip(this);
    }
}

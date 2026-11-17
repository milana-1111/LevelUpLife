namespace LevelUpLife.Core.Enums;

//Состояние привычки за конкретный день
public enum HabitStatus
{
    //Привычка запланирована на день и ещё не отмечена
    Active = 0,

    //Привычка выполнена
    Completed = 1,

    //Привычка пропущена
    Missed = 2
}

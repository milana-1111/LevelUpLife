namespace LevelUpLife.Core.Models;

//Категория привычек(спорт, учёба, режим дня и т. д.)
public class Category
{
    //Идентификатор категории
    public int Id { get; set; }

    //Название категории
    public string Name { get; set; } = string.Empty;

    //Привычки этой категории
    public List<Habit> Habits { get; set; } = new();
}

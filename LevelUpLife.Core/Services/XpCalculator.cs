using LevelUpLife.Core.Enums;

namespace LevelUpLife.Core.Services;

//Расчёт опыта (XP) за привычки и уровня пользователя.
//Уровень N открывается при XP, равном 50 · N · (N − 1):
//2-й уровень — 100 XP, 3-й — 300 XP, 4-й — 600 XP и так далее
public class XpCalculator
{
    public const int EasyXp = 10;
    public const int MediumXp = 20;
    public const int HardXp = 30;

    private const int LevelStep = 50;

    //Возвращает количество XP, которое даёт привычка указанной сложности
    public int CalculateXp(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => EasyXp,
            Difficulty.Medium => MediumXp,
            Difficulty.Hard => HardXp,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), "Неизвестная сложность привычки.")
        };
    }

    //Возвращает, сколько всего XP нужно набрать, чтобы достичь указанного уровня
    public int XpForLevel(int level)
    {
        if (level < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(level), "Уровень не может быть меньше 1.");
        }

        return LevelStep * level * (level - 1);
    }

    //Вычисляет уровень пользователя по суммарному опыту
    public int CalculateLevel(int totalXp)
    {
        if (totalXp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalXp), "Опыт не может быть отрицательным.");
        }

        int level = 1;
        while (totalXp >= XpForLevel(level + 1))
        {
            level++;
        }

        return level;
    }

    //Считает, сколько XP осталось до следующего уровня
    public int XpToNextLevel(int totalXp)
    {
        int level = CalculateLevel(totalXp);
        return XpForLevel(level + 1) - totalXp;
    }
}

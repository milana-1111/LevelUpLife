using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Services;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Модульные тесты расчёта опыта и уровней
/// </summary>
public class XpCalculatorTests
{
    private readonly XpCalculator _calculator = new();

    [Theory]
    [InlineData(Difficulty.Easy, 10)]
    [InlineData(Difficulty.Medium, 20)]
    [InlineData(Difficulty.Hard, 30)]
    public void CalculateXp_ReturnsXpForDifficulty(Difficulty difficulty, int expected)
    {
        Assert.Equal(expected, _calculator.CalculateXp(difficulty));
    }

    [Fact]
    public void CalculateXp_UnknownDifficulty_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.CalculateXp((Difficulty)0));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 100)]
    [InlineData(3, 300)]
    [InlineData(5, 1000)]
    public void XpForLevel_ReturnsThreshold(int level, int expected)
    {
        Assert.Equal(expected, _calculator.XpForLevel(level));
    }

    [Fact]
    public void XpForLevel_LevelBelowOne_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.XpForLevel(0));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(99, 1)]
    [InlineData(100, 2)]
    [InlineData(299, 2)]
    [InlineData(300, 3)]
    [InlineData(999, 4)]
    [InlineData(1000, 5)]
    public void CalculateLevel_ReturnsLevelByTotalXp(int totalXp, int expectedLevel)
    {
        Assert.Equal(expectedLevel, _calculator.CalculateLevel(totalXp));
    }

    [Fact]
    public void CalculateLevel_NegativeXp_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.CalculateLevel(-1));
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(50, 50)]
    [InlineData(100, 200)]
    public void XpToNextLevel_ReturnsRemainingXp(int totalXp, int expected)
    {
        Assert.Equal(expected, _calculator.XpToNextLevel(totalXp));
    }
}

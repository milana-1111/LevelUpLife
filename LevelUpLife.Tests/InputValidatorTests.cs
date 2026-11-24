using LevelUpLife.Core.Enums;
using LevelUpLife.Core.Validation;
using Xunit;

namespace LevelUpLife.Tests;

/// <summary>
/// Модульные тесты проверки вводимых данных.
/// </summary>
public class InputValidatorTests
{
    private readonly InputValidator _validator = new();

    [Theory]
    [InlineData("milana_1", "secret1", "Милана", true)]
    [InlineData("ab", "secret1", "Милана", false)]
    [InlineData("мила", "secret1", "Милана", false)]
    [InlineData("milana", "12345", "Милана", false)]
    [InlineData("milana", "secret1", "М", false)]
    [InlineData("", "", "", false)]
    public void ValidateRegistration_ChecksAllFields(string username, string password, string displayName, bool expected)
    {
        Assert.Equal(expected, _validator.ValidateRegistration(username, password, displayName).IsValid);
    }

    [Fact]
    public void ValidateRegistration_ReturnsErrorForEachBadField()
    {
        var result = _validator.ValidateRegistration("a", "1", "");

        Assert.Equal(3, result.Errors.Count);
    }

    [Theory]
    [InlineData("milana", "secret1", true)]
    [InlineData("", "secret1", false)]
    [InlineData("milana", "", false)]
    public void ValidateLogin_RequiresLoginAndPassword(string username, string password, bool expected)
    {
        Assert.Equal(expected, _validator.ValidateLogin(username, password).IsValid);
    }

    [Theory]
    [InlineData("Зарядка", 1, Difficulty.Easy, "1111111", true)]
    [InlineData("Зар", 1, Difficulty.Easy, "1111100", true)]
    [InlineData("ab", 1, Difficulty.Easy, "1111111", false)]
    [InlineData("Зарядка", 0, Difficulty.Easy, "1111111", false)]
    [InlineData("Зарядка", 1, Difficulty.Easy, "0000000", false)]
    [InlineData("Зарядка", 1, Difficulty.Easy, "111", false)]
    [InlineData("Зарядка", 1, Difficulty.Easy, "11111a1", false)]
    public void ValidateHabit_ChecksTitleCategoryAndDays(string title, int categoryId, Difficulty difficulty, string days, bool expected)
    {
        Assert.Equal(expected, _validator.ValidateHabit(title, categoryId, difficulty, days).IsValid);
    }

    [Fact]
    public void ValidateHabit_TitleLongerThan100_IsInvalid()
    {
        string longTitle = new('а', 101);

        Assert.False(_validator.ValidateHabit(longTitle, 1, Difficulty.Easy, "1111111").IsValid);
    }

    [Fact]
    public void ValidateHabit_UnknownDifficulty_IsInvalid()
    {
        Assert.False(_validator.ValidateHabit("Зарядка", 1, (Difficulty)0, "1111111").IsValid);
    }
}

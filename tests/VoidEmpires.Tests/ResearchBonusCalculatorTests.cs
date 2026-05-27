using VoidEmpires.Domain.Research;

namespace VoidEmpires.Tests;

public class ResearchBonusCalculatorTests
{
    [Theory]
    [InlineData(0, 1.00)]
    [InlineData(1, 1.05)]
    [InlineData(2, 1.10)]
    [InlineData(5, 1.25)]
    public void GetResourceProductionMultiplierReturnsExpectedValue(int level, double expected)
    {
        var multiplier = ResearchBonusCalculator.GetResourceProductionMultiplier(level);

        Assert.Equal((decimal)expected, multiplier);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(5, 50)]
    public void GetPlanetaryEngineeringCapacityBonusReturnsExpectedValue(int level, int expected)
    {
        var bonus = ResearchBonusCalculator.GetPlanetaryEngineeringCapacityBonus(level);

        Assert.Equal(expected, bonus);
    }

    [Fact]
    public void GetBonusReturnsCatalogBonusKeyAndValue()
    {
        var bonus = ResearchBonusCalculator.GetBonus(ResearchType.PlanetaryEngineering, 2);

        Assert.Equal(ResearchType.PlanetaryEngineering, bonus.ResearchType);
        Assert.Equal(2, bonus.Level);
        Assert.Equal("planet_capacity", bonus.BonusKey);
        Assert.Equal(20, bonus.Value);
    }
}

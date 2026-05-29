using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Tests;

public class PlanetVisualIntensityCalculatorTests
{
    private static readonly Guid PlanetId = Guid.Parse("d2cd1dc7-3c82-4f76-b6c9-b58b3063fc01");

    [Fact]
    public void CalculateReturnsZeroIntensitiesForUnownedEmptyPlanet()
    {
        var result = PlanetVisualIntensityCalculator.Calculate(new PlanetVisualIntensityInput(
            PlanetId,
            PlanetType.Terran,
            Size: 100,
            PlanetColonizationStatus.Uncolonized,
            IsOwned: false,
            BuildingCount: 0,
            TotalBuildingLevels: 0,
            UrbanBuildingLevels: 0,
            IndustrialBuildingLevels: 0,
            TerraformingBuildingLevels: 0,
            MilitaryBuildingLevels: 0,
            OrbitalGroupStrength: 0));

        Assert.Equal(0f, result.ColonizationIntensity);
        Assert.Equal(0f, result.UrbanIntensity);
        Assert.Equal(0f, result.IndustrialIntensity);
        Assert.Equal(0f, result.TerraformingIntensity);
        Assert.Equal(0f, result.MilitaryIntensity);
        Assert.Equal(0f, result.OrbitalPresenceIntensity);
    }

    [Fact]
    public void CalculateReturnsDeterministicStableVisualSeedForPlanetId()
    {
        var input = CreateOwnedInput(PlanetId);

        var first = PlanetVisualIntensityCalculator.Calculate(input);
        var second = PlanetVisualIntensityCalculator.Calculate(input);

        Assert.Equal(first.VisualSeed, second.VisualSeed);
        Assert.True(first.VisualSeed >= 0);
    }

    [Fact]
    public void CalculateScalesIntensitiesFromInputWeights()
    {
        var result = PlanetVisualIntensityCalculator.Calculate(new PlanetVisualIntensityInput(
            PlanetId,
            PlanetType.Ice,
            Size: 100,
            PlanetColonizationStatus.Colonized,
            IsOwned: true,
            BuildingCount: 10,
            TotalBuildingLevels: 20,
            UrbanBuildingLevels: 5,
            IndustrialBuildingLevels: 25,
            TerraformingBuildingLevels: 50,
            MilitaryBuildingLevels: 10,
            OrbitalGroupStrength: 25));

        Assert.Equal(0.35f, result.ColonizationIntensity, precision: 3);
        Assert.Equal(0.05f, result.UrbanIntensity, precision: 3);
        Assert.Equal(0.25f, result.IndustrialIntensity, precision: 3);
        Assert.Equal(0.50f, result.TerraformingIntensity, precision: 3);
        Assert.Equal(0.10f, result.MilitaryIntensity, precision: 3);
        Assert.Equal(0.50f, result.OrbitalPresenceIntensity, precision: 3);
    }

    [Fact]
    public void CalculateClampsIntensitiesToOne()
    {
        var result = PlanetVisualIntensityCalculator.Calculate(new PlanetVisualIntensityInput(
            PlanetId,
            PlanetType.Volcanic,
            Size: 10,
            PlanetColonizationStatus.Colonized,
            IsOwned: true,
            BuildingCount: 100,
            TotalBuildingLevels: 100,
            UrbanBuildingLevels: 100,
            IndustrialBuildingLevels: 100,
            TerraformingBuildingLevels: 100,
            MilitaryBuildingLevels: 100,
            OrbitalGroupStrength: 100));

        Assert.Equal(1f, result.ColonizationIntensity);
        Assert.Equal(1f, result.UrbanIntensity);
        Assert.Equal(1f, result.IndustrialIntensity);
        Assert.Equal(1f, result.TerraformingIntensity);
        Assert.Equal(1f, result.MilitaryIntensity);
        Assert.Equal(1f, result.OrbitalPresenceIntensity);
    }

    [Fact]
    public void CalculateRejectsEmptyPlanetId()
    {
        Assert.Throws<ArgumentException>(() => PlanetVisualIntensityCalculator.Calculate(CreateOwnedInput(Guid.Empty)));
    }

    [Fact]
    public void CalculateRejectsInvalidPlanetSize()
    {
        var input = new PlanetVisualIntensityInput(
            PlanetId,
            PlanetType.Terran,
            Size: 0,
            PlanetColonizationStatus.Colonized,
            IsOwned: true,
            BuildingCount: 1,
            TotalBuildingLevels: 1,
            UrbanBuildingLevels: 1,
            IndustrialBuildingLevels: 1,
            TerraformingBuildingLevels: 1,
            MilitaryBuildingLevels: 1,
            OrbitalGroupStrength: 1);

        Assert.Throws<ArgumentOutOfRangeException>(() => PlanetVisualIntensityCalculator.Calculate(input));
    }

    private static PlanetVisualIntensityInput CreateOwnedInput(Guid planetId) =>
        new(
            planetId,
            PlanetType.Terran,
            Size: 100,
            PlanetColonizationStatus.Colonized,
            IsOwned: true,
            BuildingCount: 1,
            TotalBuildingLevels: 1,
            UrbanBuildingLevels: 1,
            IndustrialBuildingLevels: 1,
            TerraformingBuildingLevels: 1,
            MilitaryBuildingLevels: 1,
            OrbitalGroupStrength: 1);
}

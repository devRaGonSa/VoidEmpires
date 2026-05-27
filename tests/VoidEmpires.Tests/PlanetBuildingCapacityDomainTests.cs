using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Tests;

public class PlanetBuildingCapacityDomainTests
{
    [Fact]
    public void CreateRejectsEmptyPlanetId()
    {
        Assert.Throws<ArgumentException>(() => PlanetBuildingCapacity.Create(Guid.Empty, 100));
    }

    [Fact]
    public void CreateRejectsInvalidBaseCapacity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PlanetBuildingCapacity.Create(Guid.NewGuid(), 0));
    }

    [Fact]
    public void CreateRejectsNegativeBonusCapacity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PlanetBuildingCapacity.Create(Guid.NewGuid(), 100, -1));
    }

    [Fact]
    public void TotalCapacityIncludesBonusCapacity()
    {
        var capacity = PlanetBuildingCapacity.Create(Guid.NewGuid(), 100, 25);

        Assert.Equal(125, capacity.TotalCapacity);
    }

    [Fact]
    public void CanFitReturnsTrueWhenCapacityIsAvailable()
    {
        var capacity = PlanetBuildingCapacity.Create(Guid.NewGuid(), 100, 25);

        Assert.True(capacity.CanFit(100, 25));
    }

    [Fact]
    public void CanFitReturnsFalseWhenCapacityWouldBeExceeded()
    {
        var capacity = PlanetBuildingCapacity.Create(Guid.NewGuid(), 100);

        Assert.False(capacity.CanFit(91, 10));
    }
}

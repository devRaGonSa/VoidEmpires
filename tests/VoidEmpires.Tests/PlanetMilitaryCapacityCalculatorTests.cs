using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Population;

namespace VoidEmpires.Tests;

public class PlanetMilitaryCapacityCalculatorTests
{
    [Fact]
    public void CalculateGroundForceCapacityUsesGroundMilitaryBuildingsOnly()
    {
        var profile = PlanetPopulationProfile.Create(Guid.NewGuid(), 1_000_000, 10_000, 5_000);
        var buildings = new[]
        {
            PlanetBuilding.Create(profile.PlanetId, BuildingType.MilitaryAcademy, 2, 10),
            PlanetBuilding.Create(profile.PlanetId, BuildingType.Barracks, 1, 9),
            PlanetBuilding.Create(profile.PlanetId, BuildingType.CrewAcademy, 5, 10)
        };

        var capacity = PlanetMilitaryCapacityCalculator.CalculateGroundForceCapacity(profile, buildings);

        Assert.Equal(14_500, capacity);
    }

    [Fact]
    public void CalculateShipCrewCapacityUsesSpaceMilitaryAndLogisticsBuildingsOnly()
    {
        var profile = PlanetPopulationProfile.Create(Guid.NewGuid(), 1_000_000, 10_000, 5_000);
        var buildings = new[]
        {
            PlanetBuilding.Create(profile.PlanetId, BuildingType.CrewAcademy, 2, 10),
            PlanetBuilding.Create(profile.PlanetId, BuildingType.FleetCommandCenter, 1, 14),
            PlanetBuilding.Create(profile.PlanetId, BuildingType.Shipyard, 3, 15),
            PlanetBuilding.Create(profile.PlanetId, BuildingType.Barracks, 5, 9)
        };

        var capacity = PlanetMilitaryCapacityCalculator.CalculateShipCrewCapacity(profile, buildings);

        Assert.Equal(9_500, capacity);
    }

    [Fact]
    public void LocalShipCrewCapacityDoesNotRepresentParkedForeignShips()
    {
        var profile = PlanetPopulationProfile.Create(Guid.NewGuid(), 1_000_000, 10_000, 5_000);
        var buildings = new[]
        {
            PlanetBuilding.Create(profile.PlanetId, BuildingType.CrewAcademy, 1, 10)
        };
        var buildingBonusCapacity = buildings.Sum(PlanetMilitaryCapacityCalculator.GetShipCrewCapacityBonus);
        var localCrewCapacity = PlanetMilitaryCapacityCalculator.CalculateShipCrewCapacity(profile, buildings);

        Assert.Equal(5_750, localCrewCapacity);
        Assert.True(profile.CanCrewLocallyBuiltShips(localCrewCapacity, buildingBonusCapacity));
        Assert.False(profile.CanCrewLocallyBuiltShips(localCrewCapacity + 1, buildingBonusCapacity));
    }
}

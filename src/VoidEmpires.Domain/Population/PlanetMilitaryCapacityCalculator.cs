using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Population;

public static class PlanetMilitaryCapacityCalculator
{
    public static long CalculateGroundForceCapacity(
        PlanetPopulationProfile populationProfile,
        IEnumerable<PlanetBuilding> buildings)
    {
        ArgumentNullException.ThrowIfNull(populationProfile);
        ArgumentNullException.ThrowIfNull(buildings);

        var buildingBonus = buildings.Sum(GetGroundForceCapacityBonus);

        return populationProfile.BaseRecruitablePopulation + buildingBonus;
    }

    public static long CalculateShipCrewCapacity(
        PlanetPopulationProfile populationProfile,
        IEnumerable<PlanetBuilding> buildings)
    {
        ArgumentNullException.ThrowIfNull(populationProfile);
        ArgumentNullException.ThrowIfNull(buildings);

        var buildingBonus = buildings.Sum(GetShipCrewCapacityBonus);

        return populationProfile.BaseCrewCapacity + buildingBonus;
    }

    public static long GetGroundForceCapacityBonus(PlanetBuilding building)
    {
        ArgumentNullException.ThrowIfNull(building);

        return building.BuildingType switch
        {
            BuildingType.MilitaryAcademy => 1_000L * building.Level,
            BuildingType.Barracks => 2_500L * building.Level,
            BuildingType.CommandCenter => 250L * building.Level,
            _ => 0
        };
    }

    public static long GetShipCrewCapacityBonus(PlanetBuilding building)
    {
        ArgumentNullException.ThrowIfNull(building);

        return building.BuildingType switch
        {
            BuildingType.CrewAcademy => 750L * building.Level,
            BuildingType.FleetCommandCenter => 1_500L * building.Level,
            BuildingType.Shipyard => 500L * building.Level,
            BuildingType.LogisticsHub => 250L * building.Level,
            _ => 0
        };
    }
}

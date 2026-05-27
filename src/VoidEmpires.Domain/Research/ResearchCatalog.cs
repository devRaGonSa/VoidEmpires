namespace VoidEmpires.Domain.Research;

public static class ResearchCatalog
{
    private static readonly IReadOnlyDictionary<ResearchType, ResearchDefinition> Definitions =
        new Dictionary<ResearchType, ResearchDefinition>
        {
            [ResearchType.PlanetaryEngineering] = new(ResearchType.PlanetaryEngineering, new ResearchCost(0,100,50,0), "planet_capacity"),
            [ResearchType.ResourceExtraction] = new(ResearchType.ResourceExtraction, new ResearchCost(0,120,80,0), "resource_output"),
            [ResearchType.EnergySystems] = new(ResearchType.EnergySystems, new ResearchCost(0,100,100,20), "energy_output"),
            [ResearchType.ConstructionAutomation] = new(ResearchType.ConstructionAutomation, new ResearchCost(0,200,150,50), "build_speed"),
            [ResearchType.Propulsion] = new(ResearchType.Propulsion, new ResearchCost(0,250,200,75), "fleet_speed"),
            [ResearchType.ShipWeapons] = new(ResearchType.ShipWeapons, new ResearchCost(0,300,250,50), "weapon_damage"),
            [ResearchType.Shielding] = new(ResearchType.Shielding, new ResearchCost(0,250,300,50), "shield_strength"),
            [ResearchType.Espionage] = new(ResearchType.Espionage, new ResearchCost(0,150,200,100), "intel_strength")
        };

    public static ResearchDefinition Get(ResearchType type) => Definitions[type];
}

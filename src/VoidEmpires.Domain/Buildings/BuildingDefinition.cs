namespace VoidEmpires.Domain.Buildings;

public sealed record BuildingDefinition(
    BuildingType BuildingType,
    int InitialLevel,
    int Footprint,
    ConstructionCost Cost);

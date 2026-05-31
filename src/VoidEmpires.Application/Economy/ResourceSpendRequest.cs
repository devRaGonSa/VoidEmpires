namespace VoidEmpires.Application.Economy;

public sealed record ResourceSpendRequest(
    Guid PlanetId,
    IReadOnlyCollection<ResourceCostDto> Costs);

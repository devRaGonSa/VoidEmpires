using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Application.Fleets;

public sealed record OrbitalTravelInsufficientResourceDto(
    ResourceType ResourceType,
    decimal RequiredQuantity,
    decimal AvailableQuantity);

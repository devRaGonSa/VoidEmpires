using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Application.Economy;

public sealed record ResourceCostDto(ResourceType ResourceType, decimal Quantity);

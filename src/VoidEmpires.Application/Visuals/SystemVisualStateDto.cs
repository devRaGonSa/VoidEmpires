namespace VoidEmpires.Application.Visuals;

public sealed record SystemVisualStateDto(Guid SystemId, IReadOnlyList<PlanetVisualStateDto> Planets);

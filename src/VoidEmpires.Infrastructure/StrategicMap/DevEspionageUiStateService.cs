using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class DevEspionageUiStateService(IStrategicMapService strategicMapService) : IDevEspionageUiStateService
{
    public async Task<GetDevEspionageUiStateResult> GetAsync(
        GetDevEspionageUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        var map = await strategicMapService.GetAsync(new GetStrategicMapRequest(request.CivilizationId), cancellationToken);
        var targets = map.Systems
            .SelectMany(system =>
            {
                var systemSignals = CountSystemSignals(system);
                var values = new List<DevEspionageTargetDto>
                {
                    CreateTarget("System", system.SystemId, null, system.SystemName, null, system.VisibilityLevel, system.VisibilityReason, systemSignals)
                };

                values.AddRange(system.Planets.Select(planet => CreateTarget(
                    "Planet",
                    system.SystemId,
                    planet.PlanetId,
                    system.SystemName,
                    planet.PlanetName,
                    planet.VisibilityLevel,
                    planet.VisibilityReason,
                    CountPlanetSignals(planet))));

                return values;
            })
            .OrderBy(x => x.TargetKind)
            .ThenBy(x => x.SystemName)
            .ThenBy(x => x.PlanetName)
            .ThenBy(x => x.SystemId)
            .ThenBy(x => x.PlanetId)
            .ToArray();

        var passiveSignals = map.Systems
            .SelectMany(system => CreateSignals(system).Concat(system.Planets.SelectMany(CreateSignals)))
            .ToArray();

        return new GetDevEspionageUiStateResult(
            request.CivilizationId,
            new DevEspionageOverviewDto(
                targets.Count(x => x.VisibilityLevel == MapVisibilityLevel.Owned),
                targets.Count(x => x.VisibilityLevel == MapVisibilityLevel.Visible),
                targets.Count(x => x.VisibilityReason is MapVisibilityReason.ExploredSystem or MapVisibilityReason.ExploredPlanet),
                targets.Count(x => x.VisibilityLevel == MapVisibilityLevel.Unknown),
                passiveSignals.Length),
            targets,
            passiveSignals,
            targets.FirstOrDefault(x => x.VisibilityLevel == MapVisibilityLevel.Unknown && x.HasPassiveSignals) is { } partial
                ? new DevEspionageRecommendedFocusDto(partial.SystemId, partial.PlanetId, "Relevant target remains partial and already has passive signal context.")
                : targets.FirstOrDefault(x => x.VisibilityLevel == MapVisibilityLevel.Unknown) is { } unknown
                    ? new DevEspionageRecommendedFocusDto(unknown.SystemId, unknown.PlanetId, "Relevant target remains partial and should stay under observation.")
                    : targets.FirstOrDefault(x => x.VisibilityLevel == MapVisibilityLevel.Visible && x.PlanetId.HasValue) is { } visible
                        ? new DevEspionageRecommendedFocusDto(visible.SystemId, visible.PlanetId, "Visible foreign target offers the clearest current intelligence comparison.")
                        : null,
            [
                new DevEspionageFutureActionDto("espionage.reconnaissance.create", false, "Reconnaissance remains a future placeholder and is not executable from this cockpit."),
                new DevEspionageFutureActionDto("espionage.infiltration.create", false, "Infiltration gameplay is not implemented."),
                new DevEspionageFutureActionDto("espionage.sabotage.create", false, "Sabotage gameplay is not implemented.")
            ],
            [
                $"Strategic relevance rows: {map.Systems.Count}.",
                $"Passive signals: {passiveSignals.Length}.",
                $"Diplomatic contacts: {map.DiplomaticContacts.Count}."
            ],
            [
                "Read-only development tooling only.",
                "Passive signals do not reveal hidden targets by themselves.",
                "No espionage mission creation or execution is available.",
                "No interception, fleet movement, combat, or map mutation is executed from this read model."
            ]);
    }

    private static DevEspionageTargetDto CreateTarget(
        string targetKind,
        Guid systemId,
        Guid? planetId,
        string? systemName,
        string? planetName,
        MapVisibilityLevel visibilityLevel,
        MapVisibilityReason visibilityReason,
        int passiveSignalCount) =>
        new(
            targetKind,
            systemId,
            planetId,
            systemName,
            planetName,
            visibilityLevel,
            visibilityReason,
            visibilityLevel == MapVisibilityLevel.Owned ? "Confirmed" :
            visibilityReason is MapVisibilityReason.ExploredSystem or MapVisibilityReason.ExploredPlanet ? "Known" :
            visibilityLevel == MapVisibilityLevel.Visible ? "Observed" : "Partial",
            visibilityLevel == MapVisibilityLevel.Owned ? "High" :
            passiveSignalCount > 0 ? "Medium" :
            visibilityLevel == MapVisibilityLevel.Visible ? "Medium" : "Low",
            passiveSignalCount > 0 ? $"{passiveSignalCount} passive signal rows available." : "No passive signal rows available.",
            passiveSignalCount > 0);

    private static IEnumerable<DevEspionagePassiveSignalDto> CreateSignals(StrategicMapSystemDto system)
    {
        if (system.SensorProfiles.Count > 0)
        {
            yield return new DevEspionagePassiveSignalDto(system.SystemId, null, "SensorProfile", $"{system.SensorProfiles.Count} sensor profile rows.");
        }

        if (system.DetectionCoverage.Count > 0)
        {
            yield return new DevEspionagePassiveSignalDto(system.SystemId, null, "DetectionCoverage", $"{system.DetectionCoverage.Count} detection coverage rows.");
        }

        if (system.TransferOverlays.Count > 0)
        {
            yield return new DevEspionagePassiveSignalDto(system.SystemId, null, "TransferSignal", $"{system.TransferOverlays.Count} visible transfer trajectories.");
        }
    }

    private static IEnumerable<DevEspionagePassiveSignalDto> CreateSignals(StrategicMapPlanetDto planet)
    {
        if (planet.SensorProfiles.Count > 0)
        {
            yield return new DevEspionagePassiveSignalDto(Guid.Empty, planet.PlanetId, "SensorProfile", $"{planet.SensorProfiles.Count} local sensor profile rows.");
        }

        if (planet.DetectionCoverage.Count > 0)
        {
            yield return new DevEspionagePassiveSignalDto(Guid.Empty, planet.PlanetId, "DetectionCoverage", $"{planet.DetectionCoverage.Count} local detection coverage rows.");
        }
    }

    private static int CountSystemSignals(StrategicMapSystemDto system) =>
        system.SensorProfiles.Count + system.DetectionCoverage.Count + system.TransferOverlays.Count;

    private static int CountPlanetSignals(StrategicMapPlanetDto planet) =>
        planet.SensorProfiles.Count + planet.DetectionCoverage.Count;
}

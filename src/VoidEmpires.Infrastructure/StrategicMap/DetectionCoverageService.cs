using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class DetectionCoverageService(
    VoidEmpiresDbContext dbContext,
    ISensorProfileService sensorProfileService) : IDetectionCoverageService
{
    public async Task<GetDetectionCoverageResult> GetAsync(
        GetDetectionCoverageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty) return new(request.CivilizationId, []);

        var profiles = (await sensorProfileService.GetAsync(new GetSensorProfilesRequest(request.CivilizationId), cancellationToken)).Profiles;
        var planetIds = profiles.Where(x => x.PlanetId.HasValue).Select(x => x.PlanetId!.Value).Distinct().ToArray();
        var systemIdsByPlanetId = await dbContext.Planets
            .AsNoTracking()
            .Where(x => planetIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.SolarSystemId, cancellationToken);

        var coverages = profiles
            .Select(profile => CreateCoverage(profile, systemIdsByPlanetId))
            .Where(x => x is not null)
            .Cast<DetectionCoverageDto>()
            .ToArray();

        return new GetDetectionCoverageResult(request.CivilizationId, coverages);
    }

    private static DetectionCoverageDto? CreateCoverage(
        SensorProfileDto profile,
        IReadOnlyDictionary<Guid, Guid> systemIdsByPlanetId)
    {
        Guid? sourceSystemId = profile.SolarSystemId;
        if (sourceSystemId is null &&
            profile.PlanetId is { } planetId &&
            systemIdsByPlanetId.TryGetValue(planetId, out var mappedSystemId))
        {
            sourceSystemId = mappedSystemId;
        }

        if (sourceSystemId is null) return null;

        return profile.SourceKind switch
        {
            SensorProfileSourceKind.Planet => new DetectionCoverageDto(
                profile.SourceId,
                DetectionCoverageSourceKind.Planet,
                MapClass(profile.SensorClass),
                profile.PlanetId,
                sourceSystemId,
                null,
                profile.DetectionRangeTier,
                100,
                [sourceSystemId.Value],
                "Local-system metadata only. Coverage does not reveal hidden systems, planets, targets, or ownership.",
                "Derived read-only placeholder from owned planet sensor context."),
            SensorProfileSourceKind.OrbitalGroup when profile.AssetType == SpaceAssetType.ScoutCraft => new DetectionCoverageDto(
                profile.SourceId,
                DetectionCoverageSourceKind.OrbitalGroup,
                MapClass(profile.SensorClass),
                profile.PlanetId,
                sourceSystemId,
                profile.OrbitalGroupId,
                profile.DetectionRangeTier,
                90,
                [sourceSystemId.Value],
                "Current-system metadata only. Coverage does not simulate range, reveal hidden targets, or create knowledge.",
                "Derived read-only placeholder from stationed scout orbital group context."),
            _ => null
        };
    }

    private static DetectionCoverageClass MapClass(SensorProfileClass sensorClass) => sensorClass switch
    {
        SensorProfileClass.Basic => DetectionCoverageClass.Basic,
        SensorProfileClass.Orbital => DetectionCoverageClass.Orbital,
        SensorProfileClass.DeepSpace => DetectionCoverageClass.DeepSpace,
        _ => DetectionCoverageClass.None
    };
}

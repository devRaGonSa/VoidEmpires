using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Application.StrategicMap;

public enum SensorProfileClass { None = 0, Basic = 1, Orbital = 2, DeepSpace = 3 }

public enum SensorProfileSourceKind { None = 0, Planet = 1, OrbitalGroup = 2, ExplorationKnowledge = 3 }

public sealed record SensorProfileDto(
    Guid SourceId,
    SensorProfileSourceKind SourceKind,
    SensorProfileClass SensorClass,
    Guid? PlanetId,
    Guid? SolarSystemId,
    Guid? OrbitalGroupId,
    SpaceAssetType? AssetType,
    int DetectionRangeTier,
    int ScanStrength,
    string Note);

public sealed record GetSensorProfilesRequest(Guid CivilizationId);

public sealed record GetSensorProfilesResult(
    Guid CivilizationId,
    IReadOnlyList<SensorProfileDto> Profiles);

public interface ISensorProfileService
{
    Task<GetSensorProfilesResult> GetAsync(GetSensorProfilesRequest request, CancellationToken cancellationToken = default);
}

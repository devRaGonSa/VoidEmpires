namespace VoidEmpires.Application.StrategicMap;

public enum DetectionCoverageClass { None = 0, Basic = 1, Orbital = 2, DeepSpace = 3 }

public enum DetectionCoverageSourceKind { None = 0, Planet = 1, OrbitalGroup = 2, ExplorationKnowledge = 3 }

public sealed record DetectionCoverageDto(
    Guid SourceId,
    DetectionCoverageSourceKind SourceKind,
    DetectionCoverageClass CoverageClass,
    Guid? SourcePlanetId,
    Guid? SourceSystemId,
    Guid? OrbitalGroupId,
    int DetectionRangeTier,
    int CoverageConfidencePercent,
    IReadOnlyList<Guid> CoveredSystemIds,
    string Limitations,
    string Note);

public sealed record GetDetectionCoverageRequest(Guid CivilizationId);

public sealed record GetDetectionCoverageResult(
    Guid CivilizationId,
    IReadOnlyList<DetectionCoverageDto> Coverages);

public interface IDetectionCoverageService
{
    Task<GetDetectionCoverageResult> GetAsync(
        GetDetectionCoverageRequest request,
        CancellationToken cancellationToken = default);
}

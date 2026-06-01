using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Application.StrategicMap;

public sealed record AlliancePactAllianceDto(
    Guid AllianceId,
    string Name,
    string Tag,
    AllianceStatus Status);

public sealed record AlliancePactReadinessDto(
    Guid AlliancePactId,
    AlliancePactAllianceDto SourceAlliance,
    AlliancePactAllianceDto TargetAlliance,
    AlliancePactType PactType,
    AlliancePactStatus Status,
    DateTime CreatedAtUtc);

public sealed record GetAlliancePactReadinessRequest(Guid CivilizationId);

public sealed record GetAlliancePactReadinessResult(
    Guid CivilizationId,
    IReadOnlyList<AlliancePactReadinessDto> Pacts,
    IReadOnlyList<string> Errors)
{
    public bool Succeeded => Errors.Count == 0;

    public static GetAlliancePactReadinessResult Success(Guid civilizationId, IReadOnlyList<AlliancePactReadinessDto> pacts) =>
        new(civilizationId, pacts, []);

    public static GetAlliancePactReadinessResult Invalid(Guid civilizationId, params string[] errors) =>
        new(civilizationId, [], errors);
}

public interface IAlliancePactReadinessQueryService
{
    Task<GetAlliancePactReadinessResult> GetAsync(
        GetAlliancePactReadinessRequest request,
        CancellationToken cancellationToken = default);
}

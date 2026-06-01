using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Application.StrategicMap;

public sealed record AllianceMembershipDto(
    Guid AllianceMembershipId,
    Guid AllianceId,
    Guid CivilizationId,
    AllianceMembershipStatus Status,
    AllianceMembershipRole Role,
    DateTime JoinedAtUtc);

public sealed record AllianceReadinessDto(
    Guid AllianceId,
    string Name,
    string Tag,
    AllianceStatus Status,
    DateTime CreatedAtUtc,
    AllianceMembershipDto Membership);

public sealed record GetAllianceReadinessRequest(Guid CivilizationId);

public sealed record GetAllianceReadinessResult(
    Guid CivilizationId,
    IReadOnlyList<AllianceReadinessDto> Alliances,
    IReadOnlyList<string> Errors)
{
    public bool Succeeded => Errors.Count == 0;

    public static GetAllianceReadinessResult Success(Guid civilizationId, IReadOnlyList<AllianceReadinessDto> alliances) =>
        new(civilizationId, alliances, []);

    public static GetAllianceReadinessResult Invalid(Guid civilizationId, params string[] errors) =>
        new(civilizationId, [], errors);
}

public interface IAllianceReadinessQueryService
{
    Task<GetAllianceReadinessResult> GetAsync(
        GetAllianceReadinessRequest request,
        CancellationToken cancellationToken = default);
}

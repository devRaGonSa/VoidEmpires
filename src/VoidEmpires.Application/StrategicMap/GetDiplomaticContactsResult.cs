using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Application.StrategicMap;

public sealed record DiplomaticContactDto(
    Guid DiplomaticContactId,
    Guid CivilizationId,
    Guid ContactedCivilizationId,
    DiplomaticContactStatus Status,
    DateTime DiscoveredAtUtc,
    string Source);

public sealed record GetDiplomaticContactsRequest(Guid CivilizationId);

public sealed record GetDiplomaticContactsResult(
    Guid CivilizationId,
    IReadOnlyList<DiplomaticContactDto> Contacts,
    IReadOnlyList<string> Errors)
{
    public bool Succeeded => Errors.Count == 0;

    public static GetDiplomaticContactsResult Success(Guid civilizationId, IReadOnlyList<DiplomaticContactDto> contacts) =>
        new(civilizationId, contacts, []);

    public static GetDiplomaticContactsResult Invalid(Guid civilizationId, params string[] errors) =>
        new(civilizationId, [], errors);
}

public interface IDiplomaticContactQueryService
{
    Task<GetDiplomaticContactsResult> GetAsync(
        GetDiplomaticContactsRequest request,
        CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class DevAllianceUiStateService(
    VoidEmpiresDbContext dbContext,
    IAllianceReadinessQueryService? allianceReadinessQueryService = null,
    IAlliancePactReadinessQueryService? alliancePactReadinessQueryService = null,
    IDiplomaticContactQueryService? diplomaticContactQueryService = null) : IDevAllianceUiStateService
{
    public async Task<GetDevAllianceUiStateResult> GetAsync(
        GetDevAllianceUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetDevAllianceUiStateResult(
                request.CivilizationId,
                null,
                null,
                [],
                [],
                [],
                null,
                null,
                [],
                ["Civilization id is required."]);
        }

        var identityRow = await (
            from civilization in dbContext.Set<Civilization>().AsNoTracking()
            join player in dbContext.Set<PlayerProfile>().AsNoTracking() on civilization.PlayerProfileId equals player.Id
            where civilization.Id == request.CivilizationId
            select new
            {
                civilization.Id,
                civilization.Name,
                civilization.Archetype,
                civilization.Status,
                civilization.HomePlanetId,
                civilization.PlayerProfileId,
                player.DisplayName
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (identityRow is null)
        {
            return new GetDevAllianceUiStateResult(
                request.CivilizationId,
                null,
                null,
                [],
                [],
                [],
                null,
                null,
                [],
                ["Civilization was not found."]);
        }

        var allianceReadiness = await (allianceReadinessQueryService ?? new AllianceReadinessQueryService(dbContext))
            .GetAsync(new GetAllianceReadinessRequest(request.CivilizationId), cancellationToken);
        var diplomaticContacts = await (diplomaticContactQueryService ?? new DiplomaticContactQueryService(dbContext))
            .GetAsync(new GetDiplomaticContactsRequest(request.CivilizationId), cancellationToken);
        var pactReadiness = await (alliancePactReadinessQueryService ?? new AlliancePactReadinessQueryService(dbContext))
            .GetAsync(new GetAlliancePactReadinessRequest(request.CivilizationId), cancellationToken);

        var activeAlliance = allianceReadiness.Alliances.FirstOrDefault(x => x.Membership.Status == AllianceMembershipStatus.Active);
        var activeAllianceCount = allianceReadiness.Alliances.Count(x => x.Membership.Status == AllianceMembershipStatus.Active);
        var futurePacts = BuildFuturePacts();
        var futureActions = BuildFutureActions();

        return new GetDevAllianceUiStateResult(
            request.CivilizationId,
            new DevAllianceIdentityDto(
                identityRow.Id,
                identityRow.Name,
                identityRow.Archetype,
                identityRow.Status,
                identityRow.HomePlanetId,
                identityRow.PlayerProfileId,
                identityRow.DisplayName),
            new DevAllianceStatusDto(
                activeAllianceCount > 0 ? "ReadOnly" : "None",
                activeAllianceCount > 0,
                activeAllianceCount,
                allianceReadiness.Alliances.Count,
                diplomaticContacts.Contacts.Count,
                pactReadiness.Pacts.Count(x => x.Status == AlliancePactStatus.Active),
                activeAlliance),
            diplomaticContacts.Contacts
                .Select(x => new DevAllianceContactDto(
                    x.ContactedCivilizationId,
                    x.Status,
                    x.DiscoveredAtUtc,
                    x.Source))
                .ToArray(),
            futurePacts,
            futureActions,
            new DevAllianceActionSummaryDto(
                futurePacts.Count + futureActions.Count,
                "ReadOnlyDiplomacy",
                futureActions.Any(x => x.ActionKey == "alliance.invitation.future"),
                futureActions.Any(x => x.ActionKey == "alliance.application.future"),
                futurePacts.Count > 0),
            new DevAllianceDiagnosticsDto(
                identityRow.HomePlanetId,
                identityRow.PlayerProfileId,
                allianceReadiness.Alliances.Count,
                diplomaticContacts.Contacts.Count,
                pactReadiness.Pacts.Count(x => x.Status == AlliancePactStatus.Active),
                BuildNotes(activeAllianceCount, diplomaticContacts.Contacts.Count, pactReadiness.Pacts.Count)),
            [
                "Alliance cockpit remains read-only in this phase.",
                "Alliance metadata does not grant shared visibility, shared sensor coverage, or shared fleet access.",
                "Invitations, applications, pact execution, membership changes, and messaging remain disabled placeholders.",
                "This read model does not mutate alliances, contacts, pacts, fleets, planets, or resources."
            ],
            []);
    }

    private static IReadOnlyList<DevAllianceFuturePactDto> BuildFuturePacts() =>
    [
        new("MutualDefenseIntent", false, "Future", "ReadOnlyDiplomacy"),
        new("TradeIntent", false, "Future", "ReadOnlyDiplomacy"),
        new("NonAggression", false, "Future", "ReadOnlyDiplomacy")
    ];

    private static IReadOnlyList<DevAllianceFutureActionDto> BuildFutureActions() =>
    [
        new("alliance.invitation.future", false, "Future", "ReadOnlyDiplomacy"),
        new("alliance.application.future", false, "Future", "ReadOnlyDiplomacy"),
        new("alliance.membership.future", false, "Future", "ReadOnlyDiplomacy")
    ];

    private static IReadOnlyList<string> BuildNotes(int activeAllianceCount, int contactCount, int pactCount)
    {
        var notes = new List<string>
        {
            "Development-only alliance read model.",
            "Identity comes from the requesting civilization and owning player profile."
        };

        if (activeAllianceCount == 0)
        {
            notes.Add("No active alliance membership rows were found for the requesting civilization.");
        }

        if (contactCount == 0)
        {
            notes.Add("No diplomatic contact rows were found for the requesting civilization.");
        }

        if (pactCount == 0)
        {
            notes.Add("No active alliance pact rows were found for the requesting civilization.");
        }

        return notes;
    }
}

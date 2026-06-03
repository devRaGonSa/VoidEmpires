using VoidEmpires.Application.Planets;
using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Infrastructure.Planets;

public sealed class DevDefenseUiStateService(IDevPlanetUiStateService planetUiStateService) : IDevDefenseUiStateService
{
    public async Task<GetDevDefenseUiStateResult> GetAsync(
        GetDevDefenseUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        var planetUiState = await planetUiStateService.GetAsync(
            new GetDevPlanetUiStateRequest(request.CivilizationId, request.PlanetId),
            cancellationToken);

        if (planetUiState.Planet is null)
        {
            return new GetDevDefenseUiStateResult(
                planetUiState.CivilizationId,
                planetUiState.SelectedPlanetId,
                planetUiState.KnownPlanets,
                null,
                planetUiState.Errors);
        }

        var defenseStructures = planetUiState.Planet.Buildings
            .Where(x => x.Category == BuildingCategory.Defense || x.BuildingType == BuildingType.DefenseGrid)
            .ToArray();
        var defenseOptions = planetUiState.Planet.ConstructionActions
            .Where(x => x.Category == BuildingCategory.Defense || x.BuildingType == BuildingType.DefenseGrid)
            .ToArray();
        var defenseQueue = planetUiState.Planet.ConstructionQueue
            .Where(x => x.Category == BuildingCategory.Defense || x.BuildingType == BuildingType.DefenseGrid)
            .ToArray();

        var protectionSummary = new DevDefenseProtectionSummaryDto(
            StructureCount: defenseStructures.Length,
            TotalDefenseLevel: defenseStructures.Sum(x => x.Level),
            AvailableOptionCount: defenseOptions.Count(x => x.AvailabilityStatus == "Available"),
            BlockedOptionCount: defenseOptions.Count(x => x.AvailabilityStatus != "Available"),
            QueueItemCount: defenseQueue.Length,
            DueQueueItemCount: defenseQueue.Count(x => x.IsDue));

        var notes = new List<string>
        {
            "Development-only defenses read model.",
            "This cockpit currently reflects defensive construction readiness, not combat behavior."
        };

        if (!planetUiState.Planet.IsOwnedByRequestingCivilization)
        {
            notes.Add("Selected planet is non-owned; defense management data stays hidden.");
        }

        if (planetUiState.Planet.ActionSummary.CompleteDueSupported)
        {
            notes.Add("Due construction completion remains disabled here because the backend completion route is global.");
        }

        var limitations = new List<string>
        {
            "Only DefenseGrid construction readiness is modeled here today.",
            "Shielding research is not applied as active mitigation in this cockpit.",
            "No combat, interception, damage, bombardment, or invasion behavior is exposed here."
        };

        var cockpit = new DevDefenseCockpitDto(
            planetUiState.Planet.PlanetId,
            planetUiState.Planet.PlanetName,
            planetUiState.Planet.SolarSystemId,
            planetUiState.Planet.SolarSystemName,
            planetUiState.Planet.IsOwnedByRequestingCivilization,
            planetUiState.Planet.OwnerCivilizationId,
            planetUiState.Planet.OwnerCivilizationName,
            planetUiState.Planet.ControlStatus,
            planetUiState.Planet.Stockpile,
            defenseStructures,
            defenseOptions,
            defenseQueue,
            protectionSummary,
            planetUiState.Planet.ActionSummary,
            new DevDefenseDiagnosticsDto(
                request.PlanetId,
                planetUiState.Planet.SolarSystemId,
                planetUiState.Planet.OwnerCivilizationId,
                planetUiState.Planet.Stockpile.Count > 0,
                defenseStructures.Length > 0,
                notes,
                limitations));

        return new GetDevDefenseUiStateResult(
            planetUiState.CivilizationId,
            planetUiState.SelectedPlanetId,
            planetUiState.KnownPlanets,
            cockpit,
            []);
    }
}

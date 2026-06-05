using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Rankings;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Population;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Rankings;

public sealed class DevRankingUiStateService(
    VoidEmpiresDbContext dbContext,
    IStrategicMapService strategicMapService) : IDevRankingUiStateService
{
    public async Task<GetDevRankingUiStateResult> GetAsync(
        GetDevRankingUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetDevRankingUiStateResult(
                request.CivilizationId,
                null,
                null,
                [],
                null,
                [],
                [],
                null,
                [],
                ["Civilization id is required."]);
        }

        var identity = await (
            from civilization in dbContext.Set<Civilization>().AsNoTracking()
            join player in dbContext.Set<PlayerProfile>().AsNoTracking() on civilization.PlayerProfileId equals player.Id
            where civilization.Id == request.CivilizationId
            select new DevRankingIdentityDto(
                civilization.Id,
                civilization.Name,
                player.DisplayName,
                civilization.HomePlanetId))
            .SingleOrDefaultAsync(cancellationToken);

        if (identity is null)
        {
            return new GetDevRankingUiStateResult(
                request.CivilizationId,
                null,
                null,
                [],
                null,
                [],
                [],
                null,
                [],
                ["Civilization was not found."]);
        }

        var ownedPlanetIds = await dbContext.Set<PlanetOwnership>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId && x.Status == PlanetControlStatus.Active)
            .Select(x => x.PlanetId)
            .ToArrayAsync(cancellationToken);

        var stockpiles = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.Set<PlanetResourceStockpile>()
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .ToListAsync(cancellationToken);
        var production = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.Set<PlanetProductionProfile>()
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .ToListAsync(cancellationToken);
        var buildings = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.Set<PlanetBuilding>()
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .ToListAsync(cancellationToken);
        var constructionOrders = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.Set<PlanetConstructionOrder>()
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .ToListAsync(cancellationToken);
        var populations = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.Set<PlanetPopulationProfile>()
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .ToListAsync(cancellationToken);
        var orbitalStocks = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.Set<OrbitalAssetStock>()
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .ToListAsync(cancellationToken);
        var planetaryStocks = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.Set<PlanetaryAssetStock>()
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .ToListAsync(cancellationToken);

        var researchProjects = await dbContext.Set<ResearchProject>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .ToListAsync(cancellationToken);
        var researchOrders = await dbContext.Set<ResearchOrder>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .ToListAsync(cancellationToken);
        var orbitalGroups = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .ToListAsync(cancellationToken);
        var transfers = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId && x.Status == OrbitalTransferStatus.Planned)
            .ToListAsync(cancellationToken);
        var alliances = await dbContext.Set<AllianceMembership>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId && x.Status == AllianceMembershipStatus.Active)
            .ToListAsync(cancellationToken);
        var pacts = alliances.Count == 0
            ? []
            : await dbContext.Set<AlliancePact>()
                .AsNoTracking()
                .Where(x => x.Status == AlliancePactStatus.Active)
                .ToListAsync(cancellationToken);
        var contacts = await dbContext.Set<DiplomaticContact>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .ToListAsync(cancellationToken);

        var strategicMap = await strategicMapService.GetAsync(
            new GetStrategicMapRequest(request.CivilizationId),
            cancellationToken);

        var economyScore = stockpiles.Sum(x => (int)(x.Credits + x.Metal + x.Crystal + x.Gas))
            + production.Sum(x => (int)(x.CreditsPerHour + x.MetalPerHour + x.CrystalPerHour + x.GasPerHour) * 4);
        var colonyScore = ownedPlanetIds.Length * 120
            + buildings.Sum(x => x.Level * 10)
            + constructionOrders.Count(x => x.Status == ConstructionQueueItemStatus.Completed) * 6;
        var researchScore = researchProjects.Sum(x => x.Level * 100)
            + researchOrders.Count(x => x.Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active) * 25;
        var orbitalScore = orbitalStocks.Sum(x => x.Quantity * 35)
            + orbitalGroups.Sum(x => x.Quantity * 45)
            + transfers.Count * 20;
        var defenseScore = buildings
            .Where(x => BuildingCatalog.Get(x.BuildingType).Category == BuildingCategory.Defense || x.BuildingType == BuildingType.DefenseGrid)
            .Sum(x => x.Level * 80);
        var groundScore = planetaryStocks.Sum(x => x.Quantity * 30)
            + populations.Sum(x => (int)(x.BaseRecruitablePopulation / 100m));
        var intelligenceScore = strategicMap.Systems.Count(x => x.IsVisible) * 35
            + strategicMap.Systems.Sum(x => x.SensorProfiles.Count * 12 + x.DetectionCoverage.Count * 12)
            + strategicMap.Systems.Sum(x => x.Planets.Count(planet => planet.IsVisible) * 8);
        var diplomacyScore = contacts.Count * 35 + alliances.Count * 90 + pacts.Count * 45;

        var categories = new[]
        {
            new DevRankingCategoryScoreDto("economicPower", economyScore, 30, "Owned planet reserves and production."),
            new DevRankingCategoryScoreDto("colonyDevelopment", colonyScore, 15, "Owned worlds, building levels, and completed construction history."),
            new DevRankingCategoryScoreDto("technologyProgress", researchScore, 15, "Completed research levels plus current queue pressure."),
            new DevRankingCategoryScoreDto("orbitalCapacity", orbitalScore, 15, "Orbital stock, stationed groups, and active transfer posture."),
            new DevRankingCategoryScoreDto("defensiveReadiness", defenseScore, 10, "Defense-category structures only; no combat simulation."),
            new DevRankingCategoryScoreDto("groundGarrison", groundScore, 5, "Ground stock and recruitable local capacity."),
            new DevRankingCategoryScoreDto("strategicIntelligence", intelligenceScore, 5, "Current visibility, sensors, and detection metadata."),
            new DevRankingCategoryScoreDto("diplomacy", diplomacyScore, 5, "Contacts plus active alliance and pact metadata.")
        };
        var totalPowerIndex = categories.Sum(x => x.Score);
        var demoComparisons = new[]
        {
            new DevRankingComparisonRowDto("current", identity.CivilizationName, totalPowerIndex, 0, true, true),
            new DevRankingComparisonRowDto("seed-reference", "Referencia de validacion", Math.Max(1, totalPowerIndex - Math.Max(25, totalPowerIndex / 6)), -Math.Max(25, totalPowerIndex / 6), false, true),
            new DevRankingComparisonRowDto("future-season", "Temporada futura", totalPowerIndex + Math.Max(40, totalPowerIndex / 8), Math.Max(40, totalPowerIndex / 8), false, true)
        };

        return new GetDevRankingUiStateResult(
            request.CivilizationId,
            identity,
            new DevRankingSummaryDto(
                totalPowerIndex,
                categories,
                categories.OrderByDescending(x => x.Score).First().CategoryKey),
            demoComparisons,
            new DevRankingPublicationStateDto("readOnly", false, "notPublic"),
            [
                new DevRankingFuturePlaceholderDto("leaderboard", false, "future", "notPublished"),
                new DevRankingFuturePlaceholderDto("season", false, "future", "futureSeason"),
                new DevRankingFuturePlaceholderDto("rewards", false, "future", "rewardsUnavailable")
            ],
            [
                new DevRankingDisabledActionDto("ranking.leaderboard.future", false, "notPublished"),
                new DevRankingDisabledActionDto("ranking.season.future", false, "futureSeason"),
                new DevRankingDisabledActionDto("ranking.rewards.future", false, "rewardsUnavailable")
            ],
            new DevRankingDiagnosticsDto(
                ownedPlanetIds.Length,
                strategicMap.Systems.Count(x => x.IsVisible),
                contacts.Count,
                transfers.Count,
                [
                    "Development-only ranking read model.",
                    "All scores are derived at read time from existing cockpit data.",
                    "Comparison rows are demo-only and do not represent a public ladder."
                ]),
            [
                "Ranking remains read-only and not published in this phase.",
                "No rewards, matchmaking, public profiles, or persistent score history exist.",
                "Fleet, defense, and intelligence values describe readiness only and do not predict combat outcomes."
            ],
            []);
    }
}

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class InterceptionOpportunityService(
    VoidEmpiresDbContext dbContext,
    IMapVisibilityService mapVisibilityService,
    IDetectionCoverageService detectionCoverageService,
    IFleetOperationalOverviewService fleetOperationalOverviewService) : IInterceptionOpportunityService
{
    public async Task<GetInterceptionOpportunitiesResult> GetAsync(
        GetInterceptionOpportunitiesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetInterceptionOpportunitiesResult(request.CivilizationId, []);
        }

        var activeTransfers = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .Where(x => x.Status != OrbitalTransferStatus.Completed && x.Status != OrbitalTransferStatus.Cancelled)
            .OrderBy(x => x.ArrivalAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        if (activeTransfers.Count == 0)
        {
            return new GetInterceptionOpportunitiesResult(request.CivilizationId, []);
        }

        var overview = await fleetOperationalOverviewService.GetAsync(
            new GetFleetOperationalOverviewRequest(request.CivilizationId),
            cancellationToken);
        var visibility = await mapVisibilityService.GetAsync(
            new GetMapVisibilityRequest(request.CivilizationId),
            cancellationToken);
        var detectionCoverage = await detectionCoverageService.GetAsync(
            new GetDetectionCoverageRequest(request.CivilizationId),
            cancellationToken);

        var candidateInterceptorPlanetIds = overview.Groups
            .Where(x => x.Status == OrbitalGroupStatus.Stationed && !x.HasActiveTransfer)
            .Select(x => x.CurrentPlanetId)
            .Distinct()
            .ToArray();

        var allPlanetIds = activeTransfers
            .SelectMany(x => new[] { x.OriginPlanetId, x.DestinationPlanetId })
            .Concat(candidateInterceptorPlanetIds)
            .Distinct()
            .ToArray();

        var systemIdsByPlanetId = await dbContext.Planets
            .AsNoTracking()
            .Where(x => allPlanetIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.SolarSystemId, cancellationToken);

        var visiblePlanetIds = visibility.Systems
            .SelectMany(x => x.Planets)
            .Where(x => x.IsVisible)
            .Select(x => x.PlanetId)
            .ToHashSet();
        var coveredSystemIds = detectionCoverage.Coverages
            .SelectMany(x => x.CoveredSystemIds)
            .ToHashSet();
        var interceptorContextSystemIds = candidateInterceptorPlanetIds
            .Where(systemIdsByPlanetId.ContainsKey)
            .Select(planetId => systemIdsByPlanetId[planetId])
            .ToHashSet();

        var opportunities = activeTransfers
            .Select(transfer => CreateOpportunity(
                transfer,
                request.CivilizationId,
                visiblePlanetIds,
                coveredSystemIds,
                interceptorContextSystemIds,
                systemIdsByPlanetId))
            .Where(x => x is not null)
            .Cast<InterceptionOpportunityDto>()
            .ToArray();

        return new GetInterceptionOpportunitiesResult(request.CivilizationId, opportunities);
    }

    private static InterceptionOpportunityDto? CreateOpportunity(
        OrbitalTransfer transfer,
        Guid requestingCivilizationId,
        IReadOnlySet<Guid> visiblePlanetIds,
        IReadOnlySet<Guid> coveredSystemIds,
        IReadOnlySet<Guid> interceptorContextSystemIds,
        IReadOnlyDictionary<Guid, Guid> systemIdsByPlanetId)
    {
        if (!systemIdsByPlanetId.TryGetValue(transfer.OriginPlanetId, out var originSystemId) ||
            !systemIdsByPlanetId.TryGetValue(transfer.DestinationPlanetId, out var destinationSystemId))
        {
            return null;
        }

        if (transfer.CivilizationId == requestingCivilizationId)
        {
            return new InterceptionOpportunityDto(
                transfer.Id,
                transfer.OrbitalGroupId,
                transfer.OriginPlanetId,
                transfer.DestinationPlanetId,
                transfer.AbstractDistanceUnits,
                transfer.DepartureAtUtc,
                transfer.ArrivalAtUtc,
                transfer.Status,
                InterceptionOpportunityStatus.ObservedOwnTransfer,
                [InterceptionOpportunityBlockReason.SelfObservedTransfer],
                false,
                "Observed through requesting-civilization fleet state.",
                "Own active transfers are surfaced as non-hostile readiness metadata only.");
        }

        var endpointsVisible = visiblePlanetIds.Contains(transfer.OriginPlanetId) &&
            visiblePlanetIds.Contains(transfer.DestinationPlanetId);
        if (!endpointsVisible)
        {
            return null;
        }

        var isDetected = coveredSystemIds.Contains(originSystemId) || coveredSystemIds.Contains(destinationSystemId);
        if (!isDetected)
        {
            return null;
        }

        var hasFriendlyInterceptorContext = interceptorContextSystemIds.Contains(originSystemId) ||
            interceptorContextSystemIds.Contains(destinationSystemId);

        return new InterceptionOpportunityDto(
            transfer.Id,
            transfer.OrbitalGroupId,
            transfer.OriginPlanetId,
            transfer.DestinationPlanetId,
            transfer.AbstractDistanceUnits,
            transfer.DepartureAtUtc,
            transfer.ArrivalAtUtc,
            transfer.Status,
            hasFriendlyInterceptorContext
                ? InterceptionOpportunityStatus.DetectedOpportunity
                : InterceptionOpportunityStatus.Blocked,
            hasFriendlyInterceptorContext
                ? []
                : [InterceptionOpportunityBlockReason.NoFriendlyInterceptorContext],
            hasFriendlyInterceptorContext,
            "Foreign transfer is visible within current local-system detection coverage.",
            hasFriendlyInterceptorContext
                ? "Potential interception opportunity only. Interception execution is not implemented."
                : "Detected transfer has no friendly stationed interceptor context in the observed endpoint systems.");
    }
}

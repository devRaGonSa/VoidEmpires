using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.StrategicMap;

public enum InterceptionOpportunityStatus
{
    None = 0,
    ObservedOwnTransfer = 1,
    DetectedOpportunity = 2,
    Blocked = 3
}

public enum InterceptionOpportunityBlockReason
{
    None = 0,
    SelfObservedTransfer = 1,
    NotDetected = 2,
    NoFriendlyInterceptorContext = 3
}

public sealed record InterceptionOpportunityDto(
    Guid TransferId,
    Guid OrbitalGroupId,
    Guid OriginPlanetId,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    OrbitalTransferStatus TransferStatus,
    InterceptionOpportunityStatus OpportunityStatus,
    IReadOnlyList<InterceptionOpportunityBlockReason> BlockReasons,
    bool HasFriendlyInterceptorContext,
    string DetectionNote,
    string ReadinessNote);

public sealed record InterceptionReadinessSummaryDto(
    InterceptionOpportunityStatus OpportunityStatus,
    IReadOnlyList<InterceptionOpportunityBlockReason> BlockReasons,
    bool HasFriendlyInterceptorContext,
    string DetectionNote,
    string ReadinessNote);

public sealed record GetInterceptionOpportunitiesRequest(Guid CivilizationId);

public sealed record GetInterceptionOpportunitiesResult(
    Guid CivilizationId,
    IReadOnlyList<InterceptionOpportunityDto> Opportunities);

public interface IInterceptionOpportunityService
{
    Task<GetInterceptionOpportunitiesResult> GetAsync(
        GetInterceptionOpportunitiesRequest request,
        CancellationToken cancellationToken = default);
}

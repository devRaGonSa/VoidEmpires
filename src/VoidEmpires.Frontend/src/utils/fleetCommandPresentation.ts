import type {
  CompleteOrbitalTransfersResponse,
  EstimateOrbitalTravelResponse,
  FleetCommandApiResult,
} from "../api/fleetCommandTypes";
import type { FleetActionHint, FleetGroupSummary } from "../api/fleetTypes";

type CommandTone = "neutral" | "good" | "warn";

export interface FleetCommandPresentationItem {
  key: string;
  label: string;
  tone: CommandTone;
  summary: string;
  details: string[];
}

function getActionLabel(actionKey: string, actionHints: FleetActionHint[], fallback: string) {
  return actionHints.find((hint) => hint.actionKey === actionKey)?.displayName ?? fallback;
}

export function buildFleetCommandReadiness(group: FleetGroupSummary, actionHints: FleetActionHint[]) {
  return [
    {
      key: "estimate",
      label: getActionLabel("fleet.travel.estimate", actionHints, "Travel preview"),
      tone: group.routeFuelReadiness?.canRequestTravelEstimate ? "good" : "warn",
      summary: group.routeFuelReadiness?.canRequestTravelEstimate ? "Preview ready" : "Preview blocked",
      details: [
        ...(group.routeFuelReadiness?.requiresDestination ? ["Destination context required."] : []),
        ...(group.routeFuelReadiness?.estimateRoute ? [`Route ${group.routeFuelReadiness.estimateRoute}`] : []),
        ...(group.routeFuelReadiness?.fuelReadinessPolicy ? [`Policy ${group.routeFuelReadiness.fuelReadinessPolicy}`] : []),
        ...(group.routeFuelReadiness?.notes ?? []),
      ],
    },
    {
      key: "create-transfer",
      label: getActionLabel("fleet.transfer.create", actionHints, "Create transfer"),
      tone: group.commands?.canCreateTransfer ? "good" : "warn",
      summary: group.commands?.canCreateTransfer ? "Ready to plan" : "Blocked by current fleet state",
      details: group.hasActiveTransfer ? ["Group already has an active transfer."] : [],
    },
    {
      key: "split",
      label: getActionLabel("fleet.group.split", actionHints, "Split group"),
      tone: group.commands?.canSplit ? "good" : "warn",
      summary: group.commands?.canSplit ? "Quantity can be partitioned" : "Split unavailable",
      details: group.hasActiveTransfer ? ["Active transfers prevent safe split operations."] : [],
    },
    {
      key: "merge",
      label: getActionLabel("fleet.group.merge", actionHints, "Merge groups"),
      tone: group.commands?.canMerge ? "good" : "warn",
      summary: group.commands?.canMerge ? "Compatible groups can merge" : "Merge unavailable",
      details: group.hasActiveTransfer ? ["Active transfers prevent safe merge operations."] : [],
    },
    {
      key: "cancel-transfer",
      label: getActionLabel("fleet.transfer.cancel", actionHints, "Cancel transfer"),
      tone: group.commands?.canCancelTransfer ? "good" : "neutral",
      summary: group.commands?.canCancelTransfer ? "Active transfer can be cancelled" : "No cancellable transfer",
      details: group.activeTransfer ? [`Transfer ${group.activeTransfer.id}`] : [],
    },
  ] satisfies FleetCommandPresentationItem[];
}

export function presentEstimateResult(result: FleetCommandApiResult<EstimateOrbitalTravelResponse>): FleetCommandPresentationItem {
  const response = result.response;
  const errors = response?.errors ?? [];
  const costSummary = response?.resourceCosts.length
    ? response.resourceCosts.map((cost) => `${cost.resourceType} ${cost.quantity}`).join(", ")
    : "No projected cost components.";

  return {
    key: "estimate-result",
    label: "Travel estimate result",
    tone: result.httpStatus === 200 && response?.succeeded ? (response.canAfford ? "good" : "warn") : "warn",
    summary:
      result.httpStatus === 200 && response?.succeeded
        ? `Distance ${response.abstractDistanceUnits} with ${response.estimatedDuration ?? "unknown duration"}.`
        : errors[0] ?? `Request returned ${result.httpStatus}.`,
    details: [
      costSummary,
      ...(response?.fuelReadiness ? [`Fuel ready: ${response.fuelReadiness.isFuelReady ? "yes" : "no"}`] : []),
      ...(response?.insufficientResources.map((resource) => `Missing ${resource.resourceType}: ${resource.requiredQuantity - resource.availableQuantity}`) ?? []),
      ...errors.slice(1),
    ],
  };
}

export function presentCompletionResult(result: FleetCommandApiResult<CompleteOrbitalTransfersResponse>): FleetCommandPresentationItem {
  const response = result.response;

  return {
    key: "complete-result",
    label: "Complete due transfers",
    tone: result.httpStatus === 200 && response?.succeeded ? "good" : "warn",
    summary:
      result.httpStatus === 200 && response?.succeeded
        ? `${response.completedCount} transfer${response.completedCount === 1 ? "" : "s"} completed.`
        : response?.errors[0] ?? `Request returned ${result.httpStatus}.`,
    details: [
      ...(response?.completedTransferIds.length ? [`Transfers ${response.completedTransferIds.join(", ")}`] : []),
      ...(response?.completedOrbitalGroupIds.length ? [`Groups ${response.completedOrbitalGroupIds.join(", ")}`] : []),
      ...(response?.errors.slice(1) ?? []),
    ],
  };
}

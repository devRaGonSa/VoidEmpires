import type { FleetGroupSummary } from "../api/fleetTypes";
import type { FleetCommandPresentationItem } from "../utils/fleetCommandPresentation";
import {
  formatBooleanLabel,
  formatCompactGuid,
  formatFuelReadinessPolicy,
  formatOrbitalGroupStatus,
  formatPlanetPrimaryLabel,
  formatPlanetSecondaryLabel,
  formatSpaceAssetType,
  formatTransferStatus,
} from "../utils/domainPresentation";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";
import { UiProgressBar } from "./ui/UiProgressBar";

function getTransferProgress(group: FleetGroupSummary) {
  const activeTransfer = group.activeTransfer;
  if (!activeTransfer) {
    return null;
  }

  const departure = Date.parse(activeTransfer.departureAtUtc);
  const arrival = Date.parse(activeTransfer.arrivalAtUtc);
  const now = Date.now();

  if (Number.isNaN(departure) || Number.isNaN(arrival) || arrival <= departure) {
    return null;
  }

  return Math.max(0, Math.min(100, ((now - departure) / (arrival - departure)) * 100));
}

function formatTransferProgressLabel(group: FleetGroupSummary) {
  const progress = getTransferProgress(group);
  return progress === null ? null : `${Math.round(progress)}% completado`;
}

interface FleetSelectedGroupPanelProps {
  group: FleetGroupSummary;
  readinessItems: FleetCommandPresentationItem[];
  groupTone: "good" | "warn" | "neutral";
  canCompleteDueTransfers: boolean;
  dueTransferCount: number;
  preparedCompleteDueGroupId: string;
  hasCompleteDueAcknowledgement: boolean;
  isCompletingDueTransfers: boolean;
  preparedCancelTransferId: string;
  hasCancelTransferAcknowledgement: boolean;
  isCancellingTransfer: boolean;
  onPrepareCompleteDueTransfer: (groupId: string) => void;
  onCompleteDueAcknowledgementChange: (checked: boolean) => void;
  onCompleteDueTransfers: (group: FleetGroupSummary) => void;
  onPrepareCancelTransfer: (transferId: string) => void;
  onCancelAcknowledgementChange: (checked: boolean) => void;
  onCancelTransfer: (group: FleetGroupSummary) => void;
}

export function FleetSelectedGroupPanel({
  group,
  readinessItems,
  groupTone,
  canCompleteDueTransfers,
  dueTransferCount,
  preparedCompleteDueGroupId,
  hasCompleteDueAcknowledgement,
  isCompletingDueTransfers,
  preparedCancelTransferId,
  hasCancelTransferAcknowledgement,
  isCancellingTransfer,
  onPrepareCompleteDueTransfer,
  onCompleteDueAcknowledgementChange,
  onCompleteDueTransfers,
  onPrepareCancelTransfer,
  onCancelAcknowledgementChange,
  onCancelTransfer,
}: FleetSelectedGroupPanelProps) {
  const transferProgress = getTransferProgress(group);
  const transferProgressLabel = formatTransferProgressLabel(group);
  const isDueTransferReady = Boolean(group.activeTransfer && (transferProgress ?? -1) >= 100);
  const canCancelTransfer = Boolean(group.activeTransfer?.id && group.commands?.canCancelTransfer);
  const dueTransferSummary = isDueTransferReady
    ? dueTransferCount > 1
      ? `La llegada visible ya vencio y hay ${dueTransferCount} llegadas listas para cerrar.`
      : "La llegada visible ya vencio y ya puede cerrarse desde esta pantalla."
    : group.activeTransfer
      ? "Este traslado sigue en curso. Solo podras cerrarlo cuando la llegada ya haya vencido."
      : "No hay un traslado activo seleccionado para cerrar llegadas vencidas.";

  return (
    <UiCard className="panel fleet-selected-panel">
      <div className="figma-section-header">
        <div className="fleet-identity-block">
          <p className="eyebrow">Escuadra seleccionada</p>
          <h3>{formatSpaceAssetType(group.assetType)}</h3>
          <p>Orbita en {formatPlanetPrimaryLabel(group.currentPlanetId)} y concentra posicion, disponibilidad y cualquier traslado activo.</p>
          <p className="dev-meta">ID tactico {formatCompactGuid(group.id)}</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge tone={groupTone}>{formatOrbitalGroupStatus(group.status)}</UiBadge>
          {group.routeFuelReadiness?.fuelReadinessPolicy ? (
            <UiBadge>{formatFuelReadinessPolicy(group.routeFuelReadiness.fuelReadinessPolicy)}</UiBadge>
          ) : null}
        </div>
      </div>

      <div className="figma-stat-grid">
        <div className="figma-stat">
          <strong>{group.quantity}</strong>
          <span>Cantidad</span>
        </div>
        <div className="figma-stat">
          <strong>{group.activeTransfer?.abstractDistanceUnits ?? 0}</strong>
          <span>Tramo orbital</span>
        </div>
      </div>

      <div className="fleet-selected-grid">
        <section className="subpanel figma-subpanel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Despliegue</p>
              <h4>Posicion orbital</h4>
            </div>
            <UiBadge>Estado visible</UiBadge>
          </div>
          <div className="figma-data-list">
            <div className="figma-data-row">
              <span>Planeta actual</span>
              <strong>{formatPlanetPrimaryLabel(group.currentPlanetId)}</strong>
            </div>
            <div className="figma-data-row">
              <span>Planeta de origen</span>
              <strong>{formatPlanetPrimaryLabel(group.originPlanetId)}</strong>
            </div>
            <div className="figma-data-row">
              <span>Lejos del origen</span>
              <strong>{formatBooleanLabel(group.isStationedAwayFromOrigin)}</strong>
            </div>
          </div>
          <p className="dev-meta">
            {formatPlanetSecondaryLabel(group.currentPlanetId) ?? "Sin ID actual"} | {formatPlanetSecondaryLabel(group.originPlanetId) ?? "Sin ID de origen"}
          </p>
        </section>

        <section className="subpanel figma-subpanel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Capacidad</p>
              <h4>Acciones disponibles</h4>
            </div>
            <UiBadge tone="warn">Resumen jugable</UiBadge>
          </div>
          <div className="figma-data-list">
            {readinessItems.map((item) => (
              <div key={`${group.id}-${item.key}`} className="figma-data-row">
                <span>{item.label}</span>
                <strong>{item.summary}</strong>
              </div>
            ))}
          </div>
          {readinessItems.some((item) => item.details.length > 0) ? (
            <details className="fleet-readiness-details">
              <summary>Detalles tecnicos</summary>
              <ul className="stack-list compact-list">
                {readinessItems.flatMap((item) =>
                  item.details.map((detail) => (
                    <li key={`${group.id}-${item.key}-${detail}`}>{item.label}: {detail}</li>
                  )),
                )}
              </ul>
            </details>
          ) : null}
        </section>
      </div>

      {group.activeTransfer ? (
        <div className="figma-transfer-card fleet-selected-transfer-card">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Traslado activo</p>
              <h4>Rumbo a {formatPlanetPrimaryLabel(group.activeTransfer.destinationPlanetId)}</h4>
              <p>{formatTransferStatus(group.activeTransfer.status)}. La anulacion sigue protegida y solo aparece cuando la escuadra activa esta en foco.</p>
              <p className="dev-meta">ID de traslado {formatCompactGuid(group.activeTransfer.id)}</p>
            </div>
            <UiBadge tone="warn">{formatPlanetPrimaryLabel(group.activeTransfer.destinationPlanetId)}</UiBadge>
          </div>
          {transferProgress !== null ? <UiProgressBar value={transferProgress} tone="neutral" /> : null}
          <div className="figma-data-list">
            <div className="figma-data-row">
              <span>Destino</span>
              <strong>{formatPlanetPrimaryLabel(group.activeTransfer.destinationPlanetId)}</strong>
            </div>
            <div className="figma-data-row">
              <span>Salida</span>
              <strong>{group.activeTransfer.departureAtUtc}</strong>
            </div>
            <div className="figma-data-row">
              <span>Llegada</span>
              <strong>{group.activeTransfer.arrivalAtUtc}</strong>
            </div>
            {transferProgressLabel ? (
              <div className="figma-data-row">
                <span>Avance</span>
                <strong>{transferProgressLabel}</strong>
              </div>
            ) : null}
          </div>
          {formatPlanetSecondaryLabel(group.activeTransfer.destinationPlanetId) ? (
            <p className="dev-meta">{formatPlanetSecondaryLabel(group.activeTransfer.destinationPlanetId)}</p>
          ) : null}
          <div className="transfer-confirmation-actions">
            {canCompleteDueTransfers && isDueTransferReady ? (
              <button
                type="button"
                className="prototype-control-button transfer-prepare-button"
                onClick={() => onPrepareCompleteDueTransfer(group.id)}
              >
                {preparedCompleteDueGroupId === group.id ? "Ocultar cierre" : "Cerrar llegadas"}
              </button>
            ) : null}
            {canCancelTransfer ? (
              <button
                type="button"
                className="prototype-control-button transfer-prepare-button"
                onClick={() => onPrepareCancelTransfer(group.activeTransfer?.id ?? "")}
              >
                {preparedCancelTransferId === group.activeTransfer.id ? "Ocultar cancelacion" : "Cancelar ruta"}
              </button>
            ) : null}
          </div>
          <section className="subpanel transfer-confirmation-panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Completar vencidos</p>
                <h4>Cerrar llegadas vencidas</h4>
                <p>{dueTransferSummary}</p>
                <p className="dev-meta">
                  {group.activeTransfer
                    ? `Traslado ${formatCompactGuid(group.activeTransfer.id)} | Llegada ${group.activeTransfer.arrivalAtUtc}`
                    : "Sin traslado activo"}
                </p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone="warn">Pendiente</UiBadge>
                <UiBadge tone={isDueTransferReady && canCompleteDueTransfers ? "good" : "neutral"}>
                  {isDueTransferReady && canCompleteDueTransfers ? "Lista para confirmar" : "Bloqueada"}
                </UiBadge>
              </div>
            </div>
            <div className="figma-data-list">
              <div className="figma-data-row">
                <span>Traslados vencidos visibles</span>
                <strong>{dueTransferCount}</strong>
              </div>
              <div className="figma-data-row">
                <span>Planeta destino</span>
                <strong>{formatPlanetPrimaryLabel(group.activeTransfer.destinationPlanetId)}</strong>
              </div>
            </div>
            {!canCompleteDueTransfers ? (
              <p className="figma-panel-note">Esta accion todavia no esta disponible en esta pantalla.</p>
            ) : null}
            {!isDueTransferReady ? (
              <p className="figma-panel-note">Todavia no hay una llegada vencida para habilitar este cierre.</p>
            ) : null}
            {preparedCompleteDueGroupId === group.id ? (
              <div className="transfer-confirmation-flow">
                <label className="confirmation-checkbox">
                  <input
                    type="checkbox"
                    checked={hasCompleteDueAcknowledgement}
                    onChange={(event) => onCompleteDueAcknowledgementChange(event.target.checked)}
                    disabled={!canCompleteDueTransfers || !isDueTransferReady}
                  />
                  <span>Confirmo cerrar ahora estas llegadas vencidas</span>
                </label>
                <div className="transfer-confirmation-actions">
                  <button
                    type="button"
                    onClick={() => onCompleteDueTransfers(group)}
                    disabled={
                      isCompletingDueTransfers ||
                      !canCompleteDueTransfers ||
                      !isDueTransferReady ||
                      !hasCompleteDueAcknowledgement ||
                      preparedCompleteDueGroupId !== group.id
                    }
                  >
                    {isCompletingDueTransfers ? "Cerrando..." : "Cerrar llegadas vencidas"}
                  </button>
                </div>
              </div>
            ) : null}
          </section>
          {!canCancelTransfer ? (
            <p className="figma-panel-note">La cancelacion solo aparece cuando esta ruta ya puede detenerse.</p>
          ) : null}
          {preparedCancelTransferId === group.activeTransfer.id ? (
            <section className="subpanel transfer-confirmation-panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Confirmar anulacion</p>
                  <h4>Cancelar ruta</h4>
                  <p>La anulacion sigue protegida y no reembolsa recursos ya cobrados.</p>
                  <p className="dev-meta">
                    Traslado {formatCompactGuid(group.activeTransfer.id)} | Escuadra {formatCompactGuid(group.id)}
                  </p>
                </div>
                <div className="figma-badge-row">
                  <UiBadge tone="warn">Accion protegida</UiBadge>
                  <UiBadge tone="warn">No reembolsa recursos</UiBadge>
                </div>
              </div>
              <div className="figma-data-list">
                <div className="figma-data-row">
                  <span>Origen</span>
                  <strong>{formatPlanetPrimaryLabel(group.originPlanetId)}</strong>
                </div>
                <div className="figma-data-row">
                  <span>Planeta actual</span>
                  <strong>{formatPlanetPrimaryLabel(group.currentPlanetId)}</strong>
                </div>
                <div className="figma-data-row">
                  <span>Destino</span>
                  <strong>{formatPlanetPrimaryLabel(group.activeTransfer.destinationPlanetId)}</strong>
                </div>
                <div className="figma-data-row">
                  <span>Llegada</span>
                  <strong>{group.activeTransfer.arrivalAtUtc}</strong>
                </div>
              </div>
              <div className="transfer-confirmation-flow">
                <label className="confirmation-checkbox">
                  <input
                    type="checkbox"
                    checked={hasCancelTransferAcknowledgement}
                    onChange={(event) => onCancelAcknowledgementChange(event.target.checked)}
                  />
                  <span>Confirmo la anulacion de esta ruta</span>
                </label>
                <div className="transfer-confirmation-actions">
                  <button
                    type="button"
                    onClick={() => onCancelTransfer(group)}
                    disabled={
                      isCancellingTransfer ||
                      !hasCancelTransferAcknowledgement ||
                      preparedCancelTransferId !== group.activeTransfer.id
                    }
                  >
                    {isCancellingTransfer ? "Cancelando..." : "Cancelar ruta"}
                  </button>
                </div>
              </div>
            </section>
          ) : null}
          {group.activeTransfer.interceptionReadiness?.readinessNote ? (
            <p className="figma-panel-note">{group.activeTransfer.interceptionReadiness.readinessNote}</p>
          ) : null}
        </div>
      ) : null}

      {group.routeFuelReadiness?.notes?.length ? (
        <ul className="stack-list compact-list">
          {group.routeFuelReadiness.notes.map((note) => (
            <li key={note}>{note}</li>
          ))}
        </ul>
      ) : null}
    </UiCard>
  );
}

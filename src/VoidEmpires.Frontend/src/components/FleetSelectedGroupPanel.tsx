import type { FleetGroupSummary } from "../api/fleetTypes";
import type { FleetCommandPresentationItem } from "../utils/fleetCommandPresentation";
import {
  formatBooleanLabel,
  formatCompactGuid,
  formatFuelReadinessPolicy,
  formatOrbitalGroupStatus,
  formatPlanetReference,
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
  preparedCancelTransferId: string;
  hasCancelTransferAcknowledgement: boolean;
  isCancellingTransfer: boolean;
  onPrepareCancelTransfer: (transferId: string) => void;
  onCancelAcknowledgementChange: (checked: boolean) => void;
  onCancelTransfer: (group: FleetGroupSummary) => void;
}

export function FleetSelectedGroupPanel({
  group,
  readinessItems,
  groupTone,
  preparedCancelTransferId,
  hasCancelTransferAcknowledgement,
  isCancellingTransfer,
  onPrepareCancelTransfer,
  onCancelAcknowledgementChange,
  onCancelTransfer,
}: FleetSelectedGroupPanelProps) {
  const transferProgress = getTransferProgress(group);
  const transferProgressLabel = formatTransferProgressLabel(group);

  return (
    <UiCard className="panel fleet-selected-panel">
      <div className="figma-section-header">
        <div className="fleet-identity-block">
          <p className="eyebrow">Escuadra seleccionada</p>
          <h3>Puente tactico de {formatSpaceAssetType(group.assetType)}</h3>
          <p>El foco principal de la cabina muestra posicion, disponibilidad y cualquier traslado activo.</p>
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
              <strong>{formatPlanetReference(group.currentPlanetId)}</strong>
            </div>
            <div className="figma-data-row">
              <span>Planeta de origen</span>
              <strong>{formatPlanetReference(group.originPlanetId)}</strong>
            </div>
            <div className="figma-data-row">
              <span>Lejos del origen</span>
              <strong>{formatBooleanLabel(group.isStationedAwayFromOrigin)}</strong>
            </div>
          </div>
        </section>

        <section className="subpanel figma-subpanel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Capacidad</p>
              <h4>Lectura de mando</h4>
            </div>
            <UiBadge tone="warn">Solo inspeccion</UiBadge>
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
            <ul className="stack-list compact-list">
              {readinessItems.flatMap((item) =>
                item.details.map((detail) => (
                  <li key={`${group.id}-${item.key}-${detail}`}>{item.label}: {detail}</li>
                )),
              )}
            </ul>
          ) : null}
        </section>
      </div>

      {group.activeTransfer ? (
        <div className="figma-transfer-card fleet-selected-transfer-card">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Traslado activo</p>
              <h4>{formatTransferStatus(group.activeTransfer.status)}</h4>
              <p>La anulacion sigue protegida y solo aparece cuando la escuadra activa esta en foco.</p>
            </div>
            <UiBadge tone="warn">{formatPlanetReference(group.activeTransfer.destinationPlanetId)}</UiBadge>
          </div>
          {transferProgress !== null ? <UiProgressBar value={transferProgress} tone="neutral" /> : null}
          <div className="figma-data-list">
            <div className="figma-data-row">
              <span>ID de traslado</span>
              <strong>{formatCompactGuid(group.activeTransfer.id)}</strong>
            </div>
            <div className="figma-data-row">
              <span>Destino</span>
              <strong>{formatPlanetReference(group.activeTransfer.destinationPlanetId)}</strong>
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
          <div className="transfer-confirmation-actions">
            <button
              type="button"
              className="prototype-control-button transfer-prepare-button"
              onClick={() => onPrepareCancelTransfer(group.activeTransfer?.id ?? "")}
              disabled={!group.activeTransfer?.id || !group.commands?.canCancelTransfer}
            >
              {preparedCancelTransferId === group.activeTransfer.id ? "Ocultar confirmacion" : "Preparar anulacion"}
            </button>
          </div>
          {preparedCancelTransferId === group.activeTransfer.id ? (
            <section className="subpanel transfer-confirmation-panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Accion de desarrollo</p>
                  <h4>Cancelar transferencia orbital</h4>
                  <p>La cancelacion sigue protegida y no reembolsa recursos ya cobrados.</p>
                </div>
                <div className="figma-badge-row">
                  <UiBadge tone="warn">Accion de desarrollo</UiBadge>
                  <UiBadge tone="warn">No reembolsa recursos</UiBadge>
                </div>
              </div>
              <div className="figma-data-list">
                <div className="figma-data-row">
                  <span>ID de traslado</span>
                  <strong>{formatCompactGuid(group.activeTransfer.id)}</strong>
                </div>
                <div className="figma-data-row">
                  <span>ID tactico</span>
                  <strong>{formatCompactGuid(group.id)}</strong>
                </div>
                <div className="figma-data-row">
                  <span>Origen</span>
                  <strong>{formatPlanetReference(group.originPlanetId)}</strong>
                </div>
                <div className="figma-data-row">
                  <span>Planeta actual</span>
                  <strong>{formatPlanetReference(group.currentPlanetId)}</strong>
                </div>
                <div className="figma-data-row">
                  <span>Destino</span>
                  <strong>{formatPlanetReference(group.activeTransfer.destinationPlanetId)}</strong>
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
                  <span>Requiere confirmacion explicita</span>
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
                    {isCancellingTransfer ? "Cancelando..." : "Cancelar transferencia orbital"}
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

import { Link } from "react-router-dom";
import { splitResourceDetailParts } from "../../utils/resourceDisplay";
import { UiBadge } from "./UiBadge";

export interface TopBarStatusItem {
  label: string;
  value: string;
}

export interface TopBarResourceItem {
  key: string;
  label: string;
  amount: string;
  capacity?: number | null;
  detail?: string | null;
  quantity?: number;
}

interface TopStatusBarProps {
  account?: TopBarAccountItem;
  items: TopBarStatusItem[];
  resources?: TopBarResourceItem[];
}

export interface TopBarAccountItem {
  detail: string;
  isBusy?: boolean;
  isSignedIn: boolean;
  loginUrl: string;
  registerUrl: string;
  value: string;
  onLogout: () => void;
}

export function TopStatusBar({ account, items, resources = [] }: TopStatusBarProps) {
  return (
    <div className="top-status-bar" aria-label="Estado global del imperio">
      {resources.length > 0 ? (
        <div className="top-resource-pill-row" aria-label="Recursos del planeta seleccionado">
          {resources.map((resource) => (
            <TopResourcePill key={resource.key} resource={resource} />
          ))}
        </div>
      ) : null}
      <div className="top-status-pill-row">
        {items.map((item) => (
          <section key={item.label} className="top-status-pill">
            <div className="top-status-pill-head">
              <span>{item.label}</span>
              <strong>{item.value}</strong>
            </div>
          </section>
        ))}
      </div>
      {account ? (
        <section className="top-status-pill" aria-label="Estado de cuenta">
          <div className="top-status-pill-head">
            <span>Cuenta</span>
            <strong>{account.value}</strong>
          </div>
          <small>{account.detail}</small>
          <div className="selection-chip-row">
            {account.isSignedIn ? (
              <button className="selection-chip" disabled={account.isBusy} type="button" onClick={account.onLogout}>
                {account.isBusy ? "Saliendo" : "Salir"}
              </button>
            ) : (
              <>
                <Link className="selection-chip selection-chip-active" to={account.loginUrl}>Entrar</Link>
                <Link className="selection-chip" to={account.registerUrl}>Registro</Link>
              </>
            )}
          </div>
        </section>
      ) : (
        <UiBadge tone="resource">Estado imperial</UiBadge>
      )}
    </div>
  );
}

function TopResourcePill({ resource }: { resource: TopBarResourceItem }) {
  const detailParts = splitResourceDetailParts(resource.detail);
  const utilizationPercent = getResourceCapacityPercent(resource);
  const utilizationLabel = utilizationPercent === null
    ? null
    : `${resource.label}: ${Math.round(utilizationPercent)}% de capacidad almacenada`;

  return (
    <section className="top-resource-pill">
      <div className="top-resource-pill-main">
        <span className="top-resource-pill-label">{resource.label}</span>
        <strong className="top-resource-pill-amount">{resource.amount}</strong>
      </div>
      {detailParts.length > 0 ? (
        <div className="top-resource-pill-meta">
          {detailParts.map((part) => (
            <small key={part}>{part}</small>
          ))}
        </div>
      ) : null}
      {utilizationPercent !== null ? (
        <div
          className="top-resource-pill-bar"
          role="progressbar"
          aria-label={utilizationLabel ?? undefined}
          aria-valuemin={0}
          aria-valuemax={100}
          aria-valuenow={Math.round(utilizationPercent)}
        >
          <span style={{ width: `${utilizationPercent}%` }} />
        </div>
      ) : null}
    </section>
  );
}

function getResourceCapacityPercent(resource: TopBarResourceItem) {
  if (
    typeof resource.quantity !== "number"
    || !Number.isFinite(resource.quantity)
    || typeof resource.capacity !== "number"
    || !Number.isFinite(resource.capacity)
    || resource.capacity <= 0
  ) {
    return null;
  }

  return Math.max(0, Math.min(100, (resource.quantity / resource.capacity) * 100));
}

import { Link } from "react-router-dom";
import { UiBadge } from "./UiBadge";

export interface TopBarStatusItem {
  label: string;
  value: string;
}

export interface TopBarResourceItem {
  key: string;
  label: string;
  amount: string;
  detail?: string | null;
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
            <section key={resource.key} className="top-resource-pill">
              <span>{resource.label}</span>
              <strong>{resource.amount}</strong>
              {resource.detail ? <small>{resource.detail}</small> : null}
            </section>
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

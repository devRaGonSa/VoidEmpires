import { Link } from "react-router-dom";
import { UiBadge } from "./UiBadge";

export interface TopBarStatusItem {
  label: string;
  value: string;
}

interface TopStatusBarProps {
  account?: TopBarAccountItem;
  items: TopBarStatusItem[];
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

export function TopStatusBar({ account, items }: TopStatusBarProps) {
  return (
    <div className="top-status-bar" aria-label="Estado global del imperio">
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

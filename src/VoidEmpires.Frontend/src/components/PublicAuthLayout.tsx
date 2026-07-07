import type { ReactNode } from "react";
import { Link, useLocation } from "react-router-dom";
import { buildLoginUrl, buildRegisterUrl } from "../utils/routeUrls";

interface PublicAuthLayoutProps {
  children: ReactNode;
}

export function PublicAuthLayout({ children }: PublicAuthLayoutProps) {
  const location = useLocation();
  const isLogin = location.pathname === "/login";

  return (
    <div className="public-auth-shell">
      <a className="skip-link" href="#main-content">
        Saltar al contenido principal
      </a>
      <header className="public-auth-header">
        <Link className="public-auth-brand" to="/">
          <span className="app-topbar-brand-mark">VE</span>
          <span>
            <strong>VoidEmpires</strong>
            <small>Imperio espacial persistente</small>
          </span>
        </Link>
        <nav className="public-auth-nav" aria-label="Acceso de cuenta">
          <Link className={isLogin ? "public-auth-link public-auth-link-active" : "public-auth-link"} to={buildLoginUrl()}>
            Entrar
          </Link>
          <Link className={!isLogin ? "public-auth-link public-auth-link-active" : "public-auth-link"} to={buildRegisterUrl()}>
            Crear cuenta
          </Link>
        </nav>
      </header>
      <main className="public-auth-main" id="main-content" tabIndex={-1}>
        {children}
      </main>
    </div>
  );
}

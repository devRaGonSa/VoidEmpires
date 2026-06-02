import type { ReactNode } from "react";
import { DevEndpointNotice } from "./DevEndpointNotice";
import { SidebarNav, type SidebarNavItem } from "./SidebarNav";
import { TopResourceBar, type TopResourceItem } from "./TopResourceBar";
import { UiCard } from "./UiCard";

interface AppShellProps {
  apiBaseUrl: string;
  backendProfile: string;
  children: ReactNode;
  resources: TopResourceItem[];
  sidebarItems: SidebarNavItem[];
  userLabel: string;
}

export function AppShell({
  apiBaseUrl,
  backendProfile,
  children,
  resources,
  sidebarItems,
  userLabel,
}: AppShellProps) {
  return (
    <div className="app-shell">
      <header className="app-topbar">
        <div className="app-topbar-brand">
          <span className="app-topbar-brand-mark">VE</span>
          <div>
            <strong>VoidEmpires</strong>
            <p>Prototipo de cabina alineado con Figma</p>
          </div>
        </div>
        <TopResourceBar resources={resources} userLabel={userLabel} />
      </header>

      <div className="app-shell-frame">
        <aside className="app-sidebar">
          <div className="app-sidebar-head">
            <p className="eyebrow">Superficie imperial</p>
            <h2>Mapa de mando</h2>
            <p>Solo estan habilitadas las rutas de lectura ya implementadas.</p>
          </div>
          <SidebarNav items={sidebarItems} />
        </aside>

        <div className="app-main-column">
          <section className="shell-intro-grid">
            <UiCard className="shell-intro-card">
              <p className="eyebrow">Prototipo frontend de VoidEmpires</p>
              <h1>Superficie de mando solo para desarrollo</h1>
              <p className="lede">
                Cabina alineada con Figma para inspeccionar los contratos de
                disponibilidad actuales del backend sin habilitar mutaciones de
                juego ni autenticacion de produccion.
              </p>
            </UiCard>
            <DevEndpointNotice
              apiBaseUrl={apiBaseUrl}
              backendProfile={backendProfile}
            />
          </section>

          <main className="page-frame">{children}</main>
        </div>
      </div>
    </div>
  );
}

import type { ReactNode } from "react";
import { useLocation } from "react-router-dom";
import { DevEndpointNotice } from "./DevEndpointNotice";
import { SidebarNav, type SidebarNavItem } from "./SidebarNav";
import { TopStatusBar, type TopBarStatusItem } from "./TopResourceBar";
import { UiCard } from "./UiCard";

interface AppShellProps {
  apiBaseUrl: string;
  backendProfile: string;
  children: ReactNode;
  sidebarItems: SidebarNavItem[];
  statusItems: TopBarStatusItem[];
}

export function AppShell({
  apiBaseUrl,
  backendProfile,
  children,
  sidebarItems,
  statusItems,
}: AppShellProps) {
  const location = useLocation();
  const isFleetRoute = location.pathname === "/fleets";
  const isStrategicMapRoute =
    location.pathname === "/" || location.pathname === "/galaxy";
  const isCompactIntro = isFleetRoute || isStrategicMapRoute;

  return (
    <div className="app-shell">
      <header className="app-topbar">
        <div className="app-topbar-brand">
          <span className="app-topbar-brand-mark">VE</span>
          <div>
            <strong>VoidEmpires</strong>
            <p>Prototipo jugable local</p>
          </div>
        </div>
        <TopStatusBar items={statusItems} />
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
          <section
            className={
              isCompactIntro
                ? "shell-intro-grid shell-intro-grid-compact"
                : "shell-intro-grid"
            }
          >
            <UiCard
              className={
                isCompactIntro
                  ? "shell-intro-card shell-intro-card-compact"
                  : "shell-intro-card"
              }
            >
              <div className="shell-intro-copy">
                <p className="eyebrow">Prototipo frontend de VoidEmpires</p>
                <h1>
                  {isFleetRoute
                    ? "Cabina de mando para desarrollo"
                    : isStrategicMapRoute
                      ? "Cabina estrategica de lectura"
                      : "Superficie de mando solo para desarrollo"}
                </h1>
                <p className="lede">
                  {isFleetRoute
                    ? "Ruta operativa compacta: el contexto tecnico sigue visible, pero la jugabilidad sube en la primera vista."
                    : isStrategicMapRoute
                      ? "La galaxia prioriza mapa, seleccion y contexto tactico. Los detalles tecnicos siguen disponibles, pero pasan a una segunda capa."
                      : "Cabina alineada con Figma para inspeccionar los contratos de disponibilidad actuales del backend sin habilitar mutaciones de juego ni autenticacion de produccion."}
                </p>
              </div>
            </UiCard>
            <DevEndpointNotice
              apiBaseUrl={apiBaseUrl}
              backendProfile={backendProfile}
              compact={isFleetRoute}
            />
          </section>

          <main className="page-frame">{children}</main>
        </div>
      </div>
    </div>
  );
}

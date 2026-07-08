import type { ReactNode } from "react";
import { useEffect, useMemo, useState } from "react";
import { useLocation } from "react-router-dom";
import { voidEmpiresApi } from "../../api/voidEmpiresApi";
import type { PlanetCockpitDto, PlanetResourceBalanceDto } from "../../api/planetTypes";
import { getCurrentAccountDisplay, getCurrentAccountWorldEntry } from "../../utils/currentAccountSession";
import { formatResourceLabel } from "../../utils/resourceDisplay";
import { buildLoginUrl, buildRegisterUrl } from "../../utils/routeUrls";
import { useCurrentAccountSession } from "../../utils/useCurrentAccountSession";
import { SidebarNav, type SidebarNavItem } from "./SidebarNav";
import { TopStatusBar, type TopBarResourceItem, type TopBarStatusItem } from "./TopResourceBar";
import { UiCard } from "./UiCard";

interface AppShellProps {
  children: ReactNode;
  sidebarItems: SidebarNavItem[];
  statusItems: TopBarStatusItem[];
}

export function AppShell({
  children,
  sidebarItems,
  statusItems,
}: AppShellProps) {
  const location = useLocation();
  const currentAccountSession = useCurrentAccountSession();
  const accountDisplay = getCurrentAccountDisplay(currentAccountSession);
  const worldEntry = getCurrentAccountWorldEntry(currentAccountSession.session);
  const [resourcePlanet, setResourcePlanet] = useState<PlanetCockpitDto | null>(null);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const isHomeRoute = location.pathname === "/";
  const isFleetRoute = location.pathname === "/fleets";
  const isStrategicMapRoute = location.pathname === "/galaxy";
  const shouldShowShellIntro = isHomeRoute || isFleetRoute || isStrategicMapRoute;

  const introTitle = isHomeRoute
    ? "Inicio del imperio"
    : isFleetRoute
      ? "Readiness orbital"
      : "Mapa estrategico";

  const introDescription = isHomeRoute
    ? "Resume recursos, colas y estado del planeta seleccionado."
    : isFleetRoute
      ? "Flotas muestra preparacion, carga y ordenes confirmadas para el contexto seleccionado."
      : "La galaxia prioriza mapa, seleccion y contexto tactico para continuar hacia los sistemas del imperio.";

  const isSignedIn = currentAccountSession.status === "ready";
  const selectedResourceContext = useMemo(() => {
    const searchParams = new URLSearchParams(location.search);
    const civilizationId = searchParams.get("civilizationId")?.trim() || worldEntry?.civilizationId || "";
    const planetId = searchParams.get("planetId")?.trim() || worldEntry?.planetId || null;

    return civilizationId ? { civilizationId, planetId } : null;
  }, [location.search, worldEntry?.civilizationId, worldEntry?.planetId]);
  const filteredStatusItems = useMemo(
    () => [
      ...statusItems,
      {
        label: "Planeta",
        value: isSignedIn ? accountDisplay.planetLabel : accountDisplay.statusLabel,
      },
    ],
    [accountDisplay.planetLabel, accountDisplay.statusLabel, isSignedIn, statusItems],
  );
  const resourceItems = useMemo(
    () => buildTopResourceItems(resourcePlanet),
    [resourcePlanet],
  );

  useEffect(() => {
    if (!isSignedIn || !selectedResourceContext) {
      setResourcePlanet(null);
      return;
    }

    let isCurrent = true;
    voidEmpiresApi.getPlanetUiState(selectedResourceContext.civilizationId, selectedResourceContext.planetId)
      .then((response) => {
        if (isCurrent) {
          setResourcePlanet(response.uiState?.planet ?? null);
        }
      })
      .catch(() => {
        if (isCurrent) {
          setResourcePlanet(null);
        }
      });

    return () => {
      isCurrent = false;
    };
  }, [isSignedIn, selectedResourceContext]);

  async function handleLogout() {
    setIsLoggingOut(true);
    try {
      await voidEmpiresApi.account.logout();
      await currentAccountSession.refresh();
    } finally {
      setIsLoggingOut(false);
    }
  }

  return (
    <div className="app-shell" data-layout="game">
      <a className="skip-link" href="#main-content">
        Saltar al contenido principal
      </a>
      <header className="app-topbar" aria-label="Barra superior del juego" data-resource-scope="selected-planet">
        <div className="app-topbar-brand">
          <span className="app-topbar-brand-mark">VE</span>
          <div>
            <strong>VoidEmpires</strong>
            <p>Imperio espacial persistente</p>
          </div>
        </div>
        <TopStatusBar
          account={{
            detail: accountDisplay.detailLabel,
            isBusy: isLoggingOut,
            isSignedIn,
            loginUrl: buildLoginUrl(),
            registerUrl: buildRegisterUrl(),
            value: isSignedIn ? accountDisplay.commanderLabel : accountDisplay.statusLabel,
            onLogout: handleLogout,
          }}
          items={filteredStatusItems}
          resources={resourceItems}
        />
      </header>

      <div className="app-shell-frame">
        <aside className="app-sidebar" aria-label="Navegacion de sistemas">
          <div className="app-sidebar-head">
            <p className="eyebrow">Superficie imperial</p>
            <h2>Mapa imperial</h2>
            <p>Colonias, flotas y rutas estrategicas del imperio.</p>
          </div>
          <SidebarNav accountStatus={currentAccountSession.status} items={sidebarItems} />
          <div className="app-sidebar-status" aria-label="Estado del producto">
            <span>{accountDisplay.statusLabel}</span>
            <strong>{isSignedIn ? accountDisplay.civilizationLabel : "Entrada de cuenta"}</strong>
            <p>{accountDisplay.detailLabel}</p>
          </div>
        </aside>

        <div className="app-main-column">
          {shouldShowShellIntro ? (
            <section className="shell-intro-grid shell-intro-grid-compact">
              <UiCard className="shell-intro-card shell-intro-card-compact">
                <div className="shell-intro-copy">
                  <p className="eyebrow">VoidEmpires</p>
                  <h1>{introTitle}</h1>
                  <p className="lede">{introDescription}</p>
                </div>
              </UiCard>
            </section>
          ) : null}

          <main className="page-frame" id="main-content" tabIndex={-1}>{children}</main>
        </div>
      </div>
    </div>
  );
}

function buildTopResourceItems(planet: PlanetCockpitDto | null): TopBarResourceItem[] {
  if (!planet?.isOwnedByRequestingCivilization) {
    return [];
  }

  const productionByResource = new Map<string, number>([
    ["1", planet.productionSummary?.creditsPerHour ?? 0],
    ["2", planet.productionSummary?.metalPerHour ?? 0],
    ["3", planet.productionSummary?.crystalPerHour ?? 0],
    ["4", planet.productionSummary?.gasPerHour ?? 0],
    ["Credits", planet.productionSummary?.creditsPerHour ?? 0],
    ["Metal", planet.productionSummary?.metalPerHour ?? 0],
    ["Crystal", planet.productionSummary?.crystalPerHour ?? 0],
    ["Gas", planet.productionSummary?.gasPerHour ?? 0],
  ]);

  return planet.stockpile
    .map((entry) => createTopResourceItem(entry, productionByResource))
    .filter((entry): entry is TopBarResourceItem => entry !== null);
}

function createTopResourceItem(
  entry: PlanetResourceBalanceDto,
  productionByResource: ReadonlyMap<string, number>,
): TopBarResourceItem | null {
  const key = String(entry.resourceType);
  const production = productionByResource.get(key) ?? 0;

  return {
    key,
    label: formatResourceLabel(entry.resourceType),
    amount: formatCompactNumber(entry.quantity),
    detail: formatTopResourceDetail(entry, production),
  };
}

function formatTopResourceDetail(entry: PlanetResourceBalanceDto, production: number) {
  const parts = [];
  if (typeof entry.capacity === "number" && Number.isFinite(entry.capacity)) {
    parts.push(`${formatCompactNumber(entry.capacity)} cap.`);
  }

  if (production > 0) {
    parts.push(`+${formatCompactNumber(production)}/h`);
  }

  return parts.length > 0 ? parts.join(" | ") : null;
}

function formatCompactNumber(value: number) {
  return new Intl.NumberFormat("es-ES", {
    maximumFractionDigits: value >= 100 ? 0 : 1,
  }).format(value);
}

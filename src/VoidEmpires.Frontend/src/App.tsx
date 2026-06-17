import { Suspense, lazy, useMemo } from "react";
import { Route, Routes, useLocation } from "react-router-dom";
import { appConfig } from "./config";
import { RouteLoadingFallback } from "./components/RouteLoadingFallback";
import { AppShell } from "./components/ui/AppShell";
import type { SidebarNavItem } from "./components/ui/SidebarNav";
import { specializedPlanetModuleRoutes } from "./utils/planetModuleRoutes";
import {
  buildAllianceUrl,
  buildConstructionUrl,
  buildEspionageUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildMarketUrl,
  buildPlanetUrl,
  buildRankingUrl,
  buildSpecializedModuleUrl,
} from "./utils/routeUrls";

const StrategicMapPage = lazy(async () => {
  const module = await import("./pages/StrategicMapPage");
  return { default: module.StrategicMapPage };
});

const PlanetPage = lazy(async () => {
  const module = await import("./pages/PlanetPage");
  return { default: module.PlanetPage };
});

const ConstructionPage = lazy(async () => {
  const module = await import("./pages/ConstructionPage");
  return { default: module.ConstructionPage };
});

const ResearchPage = lazy(async () => {
  const module = await import("./pages/ResearchPage");
  return { default: module.ResearchPage };
});

const ShipyardPage = lazy(async () => {
  const module = await import("./pages/ShipyardPage");
  return { default: module.ShipyardPage };
});

const FleetsPage = lazy(async () => {
  const module = await import("./pages/FleetsPage");
  return { default: module.FleetsPage };
});

const DefensesPage = lazy(async () => {
  const module = await import("./pages/DefensesPage");
  return { default: module.DefensesPage };
});

const GroundArmyPage = lazy(async () => {
  const module = await import("./pages/GroundArmyPage");
  return { default: module.GroundArmyPage };
});

const EspionagePage = lazy(async () => {
  const module = await import("./pages/EspionagePage");
  return { default: module.EspionagePage };
});

const MarketPage = lazy(async () => {
  const module = await import("./pages/MarketPage");
  return { default: module.MarketPage };
});

const AlliancePage = lazy(async () => {
  const module = await import("./pages/AlliancePage");
  return { default: module.AlliancePage };
});

const RankingPage = lazy(async () => {
  const module = await import("./pages/RankingPage");
  return { default: module.RankingPage };
});

const OnboardingPage = lazy(async () => {
  const module = await import("./pages/OnboardingPage");
  return { default: module.OnboardingPage };
});

const ModuleCabinPage = lazy(async () => {
  const module = await import("./pages/ModuleCabinPage");
  return { default: module.ModuleCabinPage };
});

export default function App() {
  const location = useLocation();
  const shellStatusItems = useMemo(() => {
    const searchParams = new URLSearchParams(location.search);
    const civilizationId = searchParams.get("civilizationId") ?? "";
    const planetId = civilizationId ? searchParams.get("planetId") : null;

    return [
      { label: "Entorno", value: "Modo Development" },
      {
        label: "Contexto",
        value: civilizationId
          ? planetId
            ? "Civilizacion y planeta en URL"
            : "Civilizacion en URL"
          : "Sin contexto seleccionado",
      },
      { label: "Sesion", value: "Navegacion local sin login de produccion" },
    ];
  }, [location.search]);

  const sidebarItems = useMemo<SidebarNavItem[]>(() => {
    const searchParams = new URLSearchParams(location.search);
    const civilizationId = searchParams.get("civilizationId") ?? "";
    const planetId = civilizationId ? searchParams.get("planetId") : null;
    const systemId = civilizationId ? searchParams.get("systemId") : null;

    return [
      { label: "Nuevo inicio", to: "/onboarding", state: "implemented" },
      { label: "Galaxia", to: buildGalaxyUrl(civilizationId, systemId, planetId), state: "implemented" },
      { label: "Planeta", to: buildPlanetUrl(civilizationId, planetId), state: "implemented" },
      { label: "Construccion", to: buildConstructionUrl(civilizationId, planetId), state: "implemented" },
      ...specializedPlanetModuleRoutes.map((route) => ({
        label: route.label,
        to: buildSpecializedModuleUrl(route.module, civilizationId, planetId),
        state: "implemented" as const,
      })),
      { label: "Flotas", to: buildFleetsUrl(civilizationId, planetId), state: "implemented" },
      { label: "Espionaje", to: buildEspionageUrl(civilizationId, systemId, planetId), state: "implemented" },
      { label: "Alianza", to: buildAllianceUrl(civilizationId), state: "readOnly" },
      { label: "Mercado", to: buildMarketUrl(civilizationId, planetId), state: "implemented" },
      { label: "Ranking", to: buildRankingUrl(civilizationId), state: "readOnly" },
    ];
  }, [location.search]);

  return (
    <AppShell
      apiBaseUrl={appConfig.apiBaseUrl}
      backendProfile={appConfig.backendProfile}
      sidebarItems={[...sidebarItems]}
      statusItems={shellStatusItems}
    >
      <Suspense fallback={<RouteLoadingFallback />}>
        <Routes>
          <Route path="/" element={<StrategicMapPage />} />
          <Route path="/galaxy" element={<StrategicMapPage />} />
          <Route path="/onboarding" element={<OnboardingPage />} />
          <Route path="/planet" element={<PlanetPage />} />
          <Route path="/construction" element={<ConstructionPage />} />
          {specializedPlanetModuleRoutes.map((route) => (
            <Route
              key={route.path}
              path={route.path}
              element={
                route.module === "Research"
                  ? <ResearchPage />
                  : route.module === "Defenses"
                    ? <DefensesPage />
                    : route.module === "GroundArmy"
                      ? <GroundArmyPage />
                      : route.module === "Shipyard"
                        ? <ShipyardPage />
                        : <ModuleCabinPage route={route} />
              }
            />
          ))}
          <Route path="/fleets" element={<FleetsPage />} />
          <Route path="/market" element={<MarketPage />} />
          <Route path="/espionage" element={<EspionagePage />} />
          <Route path="/alliance" element={<AlliancePage />} />
          <Route path="/ranking" element={<RankingPage />} />
        </Routes>
      </Suspense>
    </AppShell>
  );
}

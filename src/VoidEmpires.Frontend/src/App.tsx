import { Suspense, lazy } from "react";
import { Route, Routes } from "react-router-dom";
import { appConfig } from "./config";
import { RouteLoadingFallback } from "./components/RouteLoadingFallback";
import { AppShell } from "./components/ui/AppShell";
import { specializedPlanetModuleRoutes } from "./utils/planetPresentation";
import { buildEspionageUrl, buildGalaxyUrl, buildMarketUrl } from "./utils/routeUrls";

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

const ModuleCabinPage = lazy(async () => {
  const module = await import("./pages/ModuleCabinPage");
  return { default: module.ModuleCabinPage };
});

const sidebarItems = [
  { label: "Galaxia", to: buildGalaxyUrl(), state: "implemented" },
  { label: "Planeta", to: "/planet", state: "implemented" },
  { label: "Construccion", to: "/construction", state: "implemented" },
  ...specializedPlanetModuleRoutes.map((route) => ({ label: route.label, to: route.path, state: "implemented" as const })),
  { label: "Flotas", to: "/fleets", state: "implemented" },
  { label: "Espionaje", to: buildEspionageUrl("00000000-0000-0000-0000-000000000001"), state: "implemented" },
  { label: "Alianza", state: "future" },
  { label: "Mercado", to: buildMarketUrl("00000000-0000-0000-0000-000000000001"), state: "implemented" },
  { label: "Ranking", state: "future" },
] as const;

const resources = [
  { label: "Metal", value: "128.4k", progress: 72, tone: "metal" },
  { label: "Cristal", value: "84.1k", progress: 58, tone: "crystal" },
  { label: "Deuterio", value: "42.8k", progress: 43, tone: "deuterium" },
  { label: "Poblacion", value: "12.4M", progress: 81, tone: "neutral" },
  { label: "Energia", value: "+480", progress: 66, tone: "power" },
] as const;

export default function App() {
  return (
    <AppShell
      apiBaseUrl={appConfig.apiBaseUrl}
      backendProfile={appConfig.backendProfile}
      resources={[...resources]}
      sidebarItems={[...sidebarItems]}
      userLabel="RaulG"
    >
      <Suspense fallback={<RouteLoadingFallback />}>
        <Routes>
          <Route path="/" element={<StrategicMapPage />} />
          <Route path="/galaxy" element={<StrategicMapPage />} />
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
        </Routes>
      </Suspense>
    </AppShell>
  );
}

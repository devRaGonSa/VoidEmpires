import { Route, Routes } from "react-router-dom";
import { appConfig } from "./config";
import { AppShell } from "./components/ui/AppShell";
import { ConstructionPage } from "./pages/ConstructionPage";
import { FleetsPage } from "./pages/FleetsPage";
import { ModuleCabinPage } from "./pages/ModuleCabinPage";
import { ResearchPage } from "./pages/ResearchPage";
import { ShipyardPage } from "./pages/ShipyardPage";
import { PlanetPage } from "./pages/PlanetPage";
import { StrategicMapPage } from "./pages/StrategicMapPage";
import { specializedPlanetModuleRoutes } from "./utils/planetPresentation";
import { buildGalaxyUrl } from "./utils/routeUrls";

const sidebarItems = [
  { label: "Resumen" },
  { label: "Planeta", to: "/planet" },
  { label: "Construccion", to: "/construction" },
  ...specializedPlanetModuleRoutes.map((route) => ({ label: route.label, to: route.path })),
  { label: "Flotas", to: "/fleets" },
  { label: "Galaxia", to: buildGalaxyUrl() },
  { label: "Espionaje" },
  { label: "Alianza" },
  { label: "Mercado" },
  { label: "Ranking" },
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
                : route.module === "Shipyard"
                  ? <ShipyardPage />
                  : <ModuleCabinPage route={route} />
            }
          />
        ))}
        <Route path="/fleets" element={<FleetsPage />} />
      </Routes>
    </AppShell>
  );
}

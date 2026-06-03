import { Route, Routes } from "react-router-dom";
import { appConfig } from "./config";
import { AppShell } from "./components/ui/AppShell";
import { ConstructionPage } from "./pages/ConstructionPage";
import { FleetsPage } from "./pages/FleetsPage";
import { PlanetPage } from "./pages/PlanetPage";
import { StrategicMapPage } from "./pages/StrategicMapPage";

const sidebarItems = [
  { label: "Resumen" },
  { label: "Planeta", to: "/planet" },
  { label: "Construccion", to: "/construction" },
  { label: "Investigacion" },
  { label: "Ejercito Tierra" },
  { label: "Astillero" },
  { label: "Defensas" },
  { label: "Flotas", to: "/fleets" },
  { label: "Galaxia", to: "/" },
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
        <Route path="/planet" element={<PlanetPage />} />
        <Route path="/construction" element={<ConstructionPage />} />
        <Route path="/fleets" element={<FleetsPage />} />
      </Routes>
    </AppShell>
  );
}

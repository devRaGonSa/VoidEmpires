import { Suspense, lazy, useMemo, type ReactElement } from "react";
import { Route, Routes, useLocation } from "react-router-dom";
import { AuthRequiredState } from "./components/AuthRequiredState";
import { PublicAuthLayout } from "./components/PublicAuthLayout";
import { RouteLoadingFallback } from "./components/RouteLoadingFallback";
import { AppShell } from "./components/ui/AppShell";
import type { SidebarNavItem } from "./components/ui/SidebarNav";
import { specializedPlanetModuleRoutes, type PlanetModuleRouteInfo } from "./utils/planetModuleRoutes";
import {
  buildAllianceUrl,
  buildConstructionUrl,
  buildEspionageUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildLoginUrl,
  buildMarketUrl,
  buildPlanetUrl,
  buildRankingUrl,
  buildRegisterUrl,
  buildSpecializedModuleUrl,
} from "./utils/routeUrls";

const StrategicMapPage = lazy(async () => {
  const module = await import("./pages/StrategicMapPage");
  return { default: module.StrategicMapPage };
});

const HomePage = lazy(async () => {
  const module = await import("./pages/HomePage");
  return { default: module.HomePage };
});

const AccountSettingsPage = lazy(async () => {
  const module = await import("./pages/AccountSettingsPage");
  return { default: module.AccountSettingsPage };
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

const RegisterPage = lazy(async () => {
  const module = await import("./pages/RegisterPage");
  return { default: module.RegisterPage };
});

const OnboardingPage = lazy(async () => {
  const module = await import("./pages/OnboardingPage");
  return { default: module.OnboardingPage };
});

const LoginPage = lazy(async () => {
  const module = await import("./pages/LoginPage");
  return { default: module.LoginPage };
});

const ModuleCabinPage = lazy(async () => {
  const module = await import("./pages/ModuleCabinPage");
  return { default: module.ModuleCabinPage };
});

const publicAuthPathnames = new Set(["/login", "/register", "/registro", "/onboarding"]);

function getPlanetModuleNavState(module: PlanetModuleRouteInfo["module"]): SidebarNavItem["state"] {
  switch (module) {
    case "Research":
    case "Shipyard":
      return "playable";
    case "Defenses":
    case "GroundArmy":
      return "readiness";
    default:
      return "readiness";
  }
}

function getRouteStatusLabel(pathname: string) {
  switch (pathname) {
    case "/":
      return "Inicio";
    case "/galaxy":
      return "Galaxia";
    case "/register":
    case "/onboarding":
      return "Registro";
    case "/login":
      return "Entrar";
    case "/account-settings":
      return "Cuenta";
    case "/planet":
      return "Planeta";
    case "/construction":
      return "Construccion";
    case "/research":
      return "Investigacion";
    case "/shipyard":
      return "Astillero";
    case "/fleets":
      return "Flotas";
    case "/defenses":
      return "Defensas";
    case "/ground-army":
      return "Ejercito Tierra";
    case "/market":
      return "Mercado";
    case "/espionage":
      return "Espionaje";
    case "/alliance":
      return "Alianza";
    case "/ranking":
      return "Ranking";
    default:
      return "Cabina";
  }
}

function requireAccount(element: ReactElement) {
  return <AuthRequiredState>{element}</AuthRequiredState>;
}

export default function App() {
  const location = useLocation();
  const isPublicAuthRoute = publicAuthPathnames.has(location.pathname);
  const shellStatusItems = useMemo(() => {
    const searchParams = new URLSearchParams(location.search);
    const civilizationId = searchParams.get("civilizationId") ?? "";
    const planetId = civilizationId ? searchParams.get("planetId") : null;

    return [
      { label: "Vista", value: getRouteStatusLabel(location.pathname) },
      {
        label: "Contexto",
        value: civilizationId
          ? planetId
            ? "Planeta seleccionado"
            : "Civilizacion seleccionada"
          : "Seleccion pendiente",
      },
      { label: "Ordenes", value: "Confirmacion requerida" },
    ];
  }, [location.pathname, location.search]);

  const sidebarItems = useMemo<SidebarNavItem[]>(() => {
    const searchParams = new URLSearchParams(location.search);
    const civilizationId = searchParams.get("civilizationId") ?? "";
    const planetId = civilizationId ? searchParams.get("planetId") : null;
    const systemId = civilizationId ? searchParams.get("systemId") : null;

    return [
      { label: "Inicio", to: "/", state: "playable" },
      { label: "Registro", to: buildRegisterUrl(), state: "playable" },
      { label: "Entrar", to: buildLoginUrl(), state: "account" },
      { label: "Cuenta", to: "/account-settings", state: "account" },
      { label: "Galaxia", to: buildGalaxyUrl(civilizationId, systemId, planetId), state: "map" },
      { label: "Planeta", to: buildPlanetUrl(civilizationId, planetId), state: "playable" },
      { label: "Construccion", to: buildConstructionUrl(civilizationId, planetId), state: "playable" },
      ...specializedPlanetModuleRoutes.map((route) => ({
        label: route.label,
        to: buildSpecializedModuleUrl(route.module, civilizationId, planetId),
        state: getPlanetModuleNavState(route.module),
      })),
      { label: "Flotas", to: buildFleetsUrl(civilizationId, planetId), state: "readiness" },
      { label: "Espionaje", to: buildEspionageUrl(civilizationId, systemId, planetId), state: "readiness" },
      { label: "Alianza", to: buildAllianceUrl(civilizationId), state: "readOnly" },
      { label: "Mercado", to: buildMarketUrl(civilizationId, planetId), state: "readiness" },
      { label: "Ranking", to: buildRankingUrl(civilizationId), state: "readOnly" },
    ];
  }, [location.search]);

  const routeContent = (
    <Suspense fallback={<RouteLoadingFallback />}>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/galaxy" element={requireAccount(<StrategicMapPage />)} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/registro" element={<RegisterPage />} />
        <Route path="/onboarding" element={<OnboardingPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account-settings" element={requireAccount(<AccountSettingsPage />)} />
        <Route path="/planet" element={requireAccount(<PlanetPage />)} />
        <Route path="/construction" element={requireAccount(<ConstructionPage />)} />
        {specializedPlanetModuleRoutes.map((route) => (
          <Route
            key={route.path}
            path={route.path}
            element={requireAccount(
              route.module === "Research"
                ? <ResearchPage />
                : route.module === "Defenses"
                  ? <DefensesPage />
                  : route.module === "GroundArmy"
                    ? <GroundArmyPage />
                    : route.module === "Shipyard"
                      ? <ShipyardPage />
                      : <ModuleCabinPage route={route} />,
            )}
          />
        ))}
        <Route path="/fleets" element={requireAccount(<FleetsPage />)} />
        <Route path="/market" element={requireAccount(<MarketPage />)} />
        <Route path="/espionage" element={requireAccount(<EspionagePage />)} />
        <Route path="/alliance" element={requireAccount(<AlliancePage />)} />
        <Route path="/ranking" element={requireAccount(<RankingPage />)} />
      </Routes>
    </Suspense>
  );

  if (isPublicAuthRoute) {
    return <PublicAuthLayout>{routeContent}</PublicAuthLayout>;
  }

  return (
    <AppShell
      sidebarItems={[...sidebarItems]}
      statusItems={shellStatusItems}
    >
      {routeContent}
    </AppShell>
  );
}

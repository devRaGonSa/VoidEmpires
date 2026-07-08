import { Suspense, lazy, useMemo, type ReactElement, type ReactNode } from "react";
import { Route, Routes, useLocation } from "react-router-dom";
import { AuthRequiredState } from "./components/AuthRequiredState";
import { PublicAuthLayout } from "./components/PublicAuthLayout";
import { RouteLoadingFallback } from "./components/RouteLoadingFallback";
import { AppShell } from "./components/ui/AppShell";
import type { SidebarNavItem } from "./components/ui/SidebarNav";
import type { TopBarStatusItem } from "./components/ui/TopResourceBar";
import { getCurrentAccountWorldEntry } from "./utils/currentAccountSession";
import { isOperatorMode } from "./utils/playableSession";
import { specializedPlanetModuleRoutes, type PlanetModuleRouteInfo } from "./utils/planetModuleRoutes";
import {
  buildAllianceUrl,
  buildConstructionUrl,
  buildEspionageUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildMarketUrl,
  buildRankingUrl,
  buildSpecializedModuleUrl,
} from "./utils/routeUrls";
import { useCurrentAccountSession } from "./utils/useCurrentAccountSession";

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
const accountRequiredPathnames = new Set([
  "/",
  "/account-settings",
  "/planet",
  "/construction",
  "/galaxy",
  "/fleets",
  "/market",
  "/espionage",
  "/alliance",
  "/ranking",
]);

function isAccountRequiredPathname(pathname: string) {
  return accountRequiredPathnames.has(pathname) || specializedPlanetModuleRoutes.some((route) => route.path === pathname);
}

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
      return "Inicio";
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
      return "Vista";
  }
}

function requireAccount(element: ReactElement) {
  return <AuthRequiredState>{element}</AuthRequiredState>;
}

interface GameLayoutProps {
  children: ReactNode;
  sidebarItems: SidebarNavItem[];
  statusItems: TopBarStatusItem[];
}

function GameLayout({ children, sidebarItems, statusItems }: GameLayoutProps) {
  return (
    <AppShell
      sidebarItems={sidebarItems}
      statusItems={statusItems}
    >
      {children}
    </AppShell>
  );
}

export default function App() {
  const location = useLocation();
  const searchParams = useMemo(() => new URLSearchParams(location.search), [location.search]);
  const currentAccountSession = useCurrentAccountSession();
  const accountWorldEntry = getCurrentAccountWorldEntry(currentAccountSession.session);
  const routeCivilizationId = searchParams.get("civilizationId")?.trim() ?? "";
  const routePlanetId = searchParams.get("planetId")?.trim() ?? "";
  const routeSystemId = searchParams.get("systemId")?.trim() ?? "";
  const shellCivilizationId = routeCivilizationId || accountWorldEntry?.civilizationId || "";
  const shellPlanetId = routePlanetId || accountWorldEntry?.planetId || null;
  const shellSystemId = routeSystemId || null;
  const isPublicAuthRoute = publicAuthPathnames.has(location.pathname);
  const requiresAccount = isAccountRequiredPathname(location.pathname);
  const shouldShowPublicAccountPrompt =
    requiresAccount
    && !isOperatorMode(searchParams)
    && (currentAccountSession.status === "signedOut" || currentAccountSession.status === "error");
  const shellStatusItems = useMemo(() => {
    return [
      { label: "Vista", value: getRouteStatusLabel(location.pathname) },
      {
        label: "Seleccion",
        value: shellCivilizationId
          ? shellPlanetId
            ? "Planeta seleccionado"
            : "Civilizacion seleccionada"
          : "Seleccion pendiente",
      },
      { label: "Ordenes", value: "Confirmacion requerida" },
    ];
  }, [location.pathname, shellCivilizationId, shellPlanetId]);

  const sidebarItems = useMemo<SidebarNavItem[]>(() => {
    return [
      { label: "Inicio", to: buildHomeUrl(shellCivilizationId, shellPlanetId), state: "playable" },
      { label: "Construccion", to: buildConstructionUrl(shellCivilizationId, shellPlanetId), state: "playable" },
      ...buildOrderedPlanetModuleSidebarItems(shellCivilizationId, shellPlanetId),
      { label: "Flotas", to: buildFleetsUrl(shellCivilizationId, shellPlanetId), state: "readiness" },
      { label: "Galaxia", to: buildGalaxyUrl(shellCivilizationId, shellSystemId, shellPlanetId), state: "map" },
      { label: "Mercado", to: buildMarketUrl(shellCivilizationId, shellPlanetId), state: "readiness" },
      { label: "Alianza", to: buildAllianceUrl(shellCivilizationId), state: "readOnly" },
      { label: "Ranking", to: buildRankingUrl(shellCivilizationId), state: "readOnly" },
      { label: "Espionaje", to: buildEspionageUrl(shellCivilizationId, shellSystemId, shellPlanetId), state: "readiness" },
    ];
  }, [shellCivilizationId, shellPlanetId, shellSystemId]);

  const routeContent = (
    <Suspense fallback={<RouteLoadingFallback />}>
      <Routes>
        <Route path="/" element={requireAccount(<HomePage />)} />
        <Route path="/galaxy" element={requireAccount(<StrategicMapPage />)} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/registro" element={<RegisterPage />} />
        <Route path="/onboarding" element={<OnboardingPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account-settings" element={requireAccount(<AccountSettingsPage />)} />
        <Route path="/planet" element={requireAccount(<HomePage />)} />
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

  if (isPublicAuthRoute || shouldShowPublicAccountPrompt) {
    return <PublicAuthLayout>{routeContent}</PublicAuthLayout>;
  }

  return (
    <GameLayout
      sidebarItems={sidebarItems}
      statusItems={shellStatusItems}
    >
      {routeContent}
    </GameLayout>
  );
}

function buildHomeUrl(civilizationId: string, planetId: string | null) {
  const searchParams = new URLSearchParams();
  if (civilizationId.trim()) {
    searchParams.set("civilizationId", civilizationId.trim());
  }

  if (planetId?.trim()) {
    searchParams.set("planetId", planetId.trim());
  }

  const query = searchParams.toString();
  return query ? `/?${query}` : "/";
}

function buildOrderedPlanetModuleSidebarItems(civilizationId: string, planetId: string | null): SidebarNavItem[] {
  const moduleOrder: PlanetModuleRouteInfo["module"][] = ["Research", "Shipyard", "Defenses", "GroundArmy"];

  return moduleOrder
    .map((module) => specializedPlanetModuleRoutes.find((route) => route.module === module))
    .filter((route): route is PlanetModuleRouteInfo => Boolean(route))
    .map((route) => ({
      label: route.label,
      to: buildSpecializedModuleUrl(route.module, civilizationId, planetId),
      state: getPlanetModuleNavState(route.module),
    }));
}

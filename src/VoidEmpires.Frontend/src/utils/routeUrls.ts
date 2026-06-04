import type { PlanetModule } from "../api/planetTypes";

const developmentSeedPlanetId = "40000000-0000-0000-0000-000000000001";

function buildUrl(path: string, params: Record<string, string | null | undefined>) {
  const searchParams = new URLSearchParams();

  for (const [key, value] of Object.entries(params)) {
    const trimmed = value?.trim();
    if (trimmed) {
      searchParams.set(key, trimmed);
    }
  }

  const query = searchParams.toString();
  return query.length > 0 ? `${path}?${query}` : path;
}

export function buildPlanetUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/planet", { civilizationId, planetId });
}

export function buildConstructionUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/construction", { civilizationId, planetId });
}

export function buildResearchUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/research", { civilizationId, planetId });
}

export function buildGroundArmyUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/ground-army", { civilizationId, planetId });
}

export function buildShipyardUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/shipyard", { civilizationId, planetId });
}

export function buildMarketUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/market", { civilizationId, planetId });
}

export function buildDefensesUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/defenses", { civilizationId, planetId });
}

export function buildGalaxyUrl(
  civilizationId?: string | null,
  systemId?: string | null,
  planetId?: string | null,
) {
  return buildUrl("/galaxy", { civilizationId, systemId, planetId });
}

export function buildFleetsUrl(civilizationId: string, planetId?: string | null) {
  return buildUrl("/fleets", { civilizationId, planetId });
}

export function buildEspionageUrl(
  civilizationId: string,
  systemId?: string | null,
  planetId?: string | null,
) {
  return buildUrl("/espionage", { civilizationId, systemId, planetId });
}

export function buildSpecializedModuleUrl(
  module: PlanetModule,
  civilizationId: string,
  planetId?: string | null,
) {
  switch (module) {
    case "Research":
      return buildResearchUrl(civilizationId, planetId);
    case "GroundArmy":
      return buildGroundArmyUrl(civilizationId, planetId);
    case "Shipyard":
      return buildShipyardUrl(civilizationId, planetId);
    case "Defenses":
      return buildDefensesUrl(civilizationId, planetId);
    default:
      return buildPlanetUrl(civilizationId, planetId);
  }
}

export function isSuspiciousCabinContext(
  civilizationId: string | null | undefined,
  planetId: string | null | undefined,
) {
  const trimmedCivilizationId = civilizationId?.trim() ?? "";
  const trimmedPlanetId = planetId?.trim() ?? "";

  return trimmedCivilizationId.startsWith("40000000")
    && (!trimmedPlanetId || trimmedPlanetId === trimmedCivilizationId);
}

export function buildDevelopmentHelperUrl() {
  return buildFleetsUrl(developmentSeedPlanetId, developmentSeedPlanetId);
}

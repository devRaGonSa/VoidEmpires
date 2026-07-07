import { FormEvent, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { isOperatorMode, savePlayableSession } from "../utils/playableSession";
import {
  buildConstructionUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  buildResearchUrl,
  buildShipyardUrl,
} from "../utils/routeUrls";

interface CreatedPlayableStart {
  civilizationId: string;
  planetId: string;
  playerDisplayName: string;
  civilizationName: string;
  planetName: string;
  planetUrl: string;
  constructionUrl: string;
  researchUrl: string;
  shipyardUrl: string;
  storedLocally: boolean;
}

const accountAuthReadinessRows = [
  {
    label: "Cuenta",
    value: "Registro y confirmacion quedan fuera de esta ruta",
    detail: "Esta pantalla solo prepara una partida local.",
  },
  {
    label: "Partida local",
    value: "Sesion de juego local",
    detail: "Conserva el contexto de navegacion para reabrir cabinas; no guarda credenciales.",
  },
  {
    label: "Propiedad",
    value: "Revalidada por cada lectura",
    detail: "Las cabinas vuelven a consultar el estado autoritativo antes de mostrar datos de juego.",
  },
] as const;

const defaultOnboardingLimitations = [
  "La partida abre una colonia inicial lista para administrar.",
  "Las cabinas conservan el contexto de juego al navegar.",
  "Las ordenes importantes siguen pidiendo confirmacion explicita.",
] as const;

function toSpanishOnboardingError(message: string) {
  switch (message.trim()) {
    case "Display name is required.":
      return "El nombre del jugador es obligatorio.";
    case "Civilization name is required.":
      return "El nombre de la civilizacion es obligatorio.";
    case "Home planet name is too long.":
      return "El nombre del planeta inicial es demasiado largo.";
    case "Display name is already in use.":
      return "Ese nombre de jugador ya esta en uso.";
    case "Civilization name is already in use.":
      return "Ese nombre de civilizacion ya esta en uso.";
    case "Persistence is not configured.":
      return "No se puede preparar la partida ahora mismo.";
    default:
      return message;
  }
}

function toProductOnboardingLimitation(message: string) {
  const normalized = toSpanishOnboardingError(message);
  const lower = normalized.toLowerCase();

  if (lower.includes("desarrollo")) {
    return "La partida usa un contexto local preparado para esta cabina.";
  }

  if (lower.includes("sesion") || lower.includes("auth")) {
    return "La partida local solo conserva la navegacion en este navegador.";
  }

  if (normalized.includes("civilizationId") || normalized.includes("planetId")) {
    return "Las cabinas conservan el contexto de juego al navegar.";
  }

  return normalized;
}

export function OnboardingPage() {
  const [searchParams] = useSearchParams();
  const operatorMode = isOperatorMode(searchParams);
  const [displayName, setDisplayName] = useState("");
  const [civilizationName, setCivilizationName] = useState("");
  const [homePlanetName, setHomePlanetName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [technicalDetail, setTechnicalDetail] = useState<string | null>(null);
  const [createdStart, setCreatedStart] = useState<CreatedPlayableStart | null>(null);
  const [limitations, setLimitations] = useState<string[]>([...defaultOnboardingLimitations]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    setTechnicalDetail(null);
    setCreatedStart(null);

    try {
      const submittedDisplayName = displayName.trim();
      const submittedCivilizationName = civilizationName.trim();
      const result = await voidEmpiresApi.createPlayableStart({
        displayName: submittedDisplayName,
        civilizationName: submittedCivilizationName,
        ...(homePlanetName.trim() ? { homePlanetName: homePlanetName.trim() } : {}),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        const backendError = result.response?.errors[0] ?? "No se pudo crear el inicio jugable.";
        setError(toSpanishOnboardingError(backendError));
        setTechnicalDetail(`Backend response: HTTP ${result.httpStatus}${result.bodyParseFailed ? " sin cuerpo JSON legible." : "."}`);
        return;
      }

      setLimitations(
        result.response.limitations.length > 0
          ? result.response.limitations.map(toProductOnboardingLimitation)
          : [...defaultOnboardingLimitations],
      );

      if (!result.response.civilizationId || !result.response.homePlanetId) {
        setError("La partida se preparo, pero no se pudo abrir Planeta automaticamente.");
        setTechnicalDetail("Faltan civilizationId o homePlanetId en la respuesta.");
        return;
      }

      const planetName = result.response.homePlanetName ?? (homePlanetName.trim() || "Planeta inicial");
      const storedSession = savePlayableSession({
        civilizationId: result.response.civilizationId,
        planetId: result.response.homePlanetId,
        playerDisplayName: submittedDisplayName,
        civilizationName: submittedCivilizationName,
        planetName,
      });

      setCreatedStart({
        civilizationId: result.response.civilizationId,
        planetId: result.response.homePlanetId,
        playerDisplayName: submittedDisplayName,
        civilizationName: submittedCivilizationName,
        planetName,
        planetUrl: buildPlanetUrl(result.response.civilizationId, result.response.homePlanetId),
        constructionUrl: buildConstructionUrl(result.response.civilizationId, result.response.homePlanetId),
        researchUrl: buildResearchUrl(result.response.civilizationId, result.response.homePlanetId),
        shipyardUrl: buildShipyardUrl(result.response.civilizationId, result.response.homePlanetId),
        storedLocally: storedSession !== null,
      });

      if (!storedSession) {
        setTechnicalDetail("El inicio fue creado, pero el navegador no permitio guardar la partida local.");
      }
    } catch (requestError) {
      setError("No se pudo preparar la partida inicial. Reintenta en unos momentos.");
      setTechnicalDetail(requestError instanceof Error ? requestError.message : "Error desconocido.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Nueva partida"
        title="Comenzar partida"
        description="Crea tu comandante, nombra una civilizacion y funda el mundo inicial desde el que administraras el imperio."
        developmentNote="La partida se abre en Planeta cuando la colonia inicial queda lista."
        badges={
          <>
            <UiBadge tone="good">Nuevo imperio</UiBadge>
            <UiBadge>Colonia inicial</UiBadge>
            <UiBadge>Confirmacion local</UiBadge>
          </>
        }
      />

      {operatorMode ? (
      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Cuenta y acceso</p>
            <h3>Frontera actual de autenticacion</h3>
          </div>
          <UiBadge tone="warn">Preparada, no productizada</UiBadge>
        </div>
        <p className="figma-panel-note">
          VoidEmpires mantiene cuenta y confirmacion separadas de esta ruta. Esta pantalla solo prepara una partida local.
        </p>
        <div className="figma-data-list">
          {accountAuthReadinessRows.map((item) => (
            <div className="figma-data-row" key={item.label}>
              <span>{item.label}</span>
              <strong>{item.value}</strong>
              <small>{item.detail}</small>
            </div>
          ))}
        </div>
      </UiCard>
      ) : null}

      <UiCard className="panel strategic-loader-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Nueva partida</p>
            <h3>Preparar comandante y mundo inicial</h3>
          </div>
          <UiBadge tone="good">Comienzo</UiBadge>
        </div>
        <p className="figma-panel-note">
          Elige los nombres principales y abre tu primera colonia con una ruta lista hacia Planeta, Construccion, Investigacion y Astillero.
        </p>

        <form className="query-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Crear comandante</span>
            <input
              type="text"
              value={displayName}
              onChange={(event) => setDisplayName(event.target.value)}
              placeholder="Comandante Vega"
              maxLength={128}
            />
          </label>
          <label className="field">
            <span>Nombrar civilizacion</span>
            <input
              type="text"
              value={civilizationName}
              onChange={(event) => setCivilizationName(event.target.value)}
              placeholder="Dominio Solar"
              maxLength={128}
            />
          </label>
          <label className="field">
            <span>Fundar mundo inicial</span>
            <input
              type="text"
              value={homePlanetName}
              onChange={(event) => setHomePlanetName(event.target.value)}
              placeholder="Nova Prime"
              maxLength={128}
            />
          </label>
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Fundando mundo..." : "Comenzar partida"}
          </button>
        </form>

        {error ? <p className="error-text">{error}</p> : null}
        {operatorMode && technicalDetail ? (
          <details className="technical-disclosure">
            <summary>Detalle tecnico de operador</summary>
            <p className="figma-panel-note">{technicalDetail}</p>
          </details>
        ) : null}
        {error ? (
          <p className="figma-panel-note">
            Revisa los nombres elegidos y vuelve a intentarlo.
          </p>
        ) : null}

        {createdStart ? (
          <div className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Inicio creado</p>
                <h3>{createdStart.planetName}</h3>
              </div>
              <UiBadge tone="good">Contexto listo</UiBadge>
            </div>
            <p className="figma-panel-note">
              La colonia inicial esta lista. La partida local conserva los enlaces de navegacion para este navegador.
            </p>
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={createdStart.planetUrl}>
                Abrir Planeta
              </Link>
              <Link className="selection-chip" to={createdStart.constructionUrl}>
                Construccion
              </Link>
              <Link className="selection-chip" to={createdStart.researchUrl}>
                Investigacion
              </Link>
              <Link className="selection-chip" to={createdStart.shipyardUrl}>
                Astillero
              </Link>
            </div>
            {operatorMode ? (
            <details className="technical-disclosure">
              <summary>Detalles de navegacion local</summary>
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Jugador</span><strong>{createdStart.playerDisplayName}</strong></div>
                <div className="figma-data-row"><span>Civilizacion</span><strong>{createdStart.civilizationName}</strong></div>
                <div className="figma-data-row"><span>Planeta</span><strong>{createdStart.planetName}</strong></div>
                <div className="figma-data-row"><span>Partida local</span><strong>{createdStart.storedLocally ? "Guardada" : "No disponible"}</strong></div>
                <div className="figma-data-row"><span>civilizationId</span><strong>{createdStart.civilizationId}</strong></div>
                <div className="figma-data-row"><span>planetId</span><strong>{createdStart.planetId}</strong></div>
              </div>
            </details>
            ) : null}
          </div>
        ) : null}

        <div className="selection-chip-row">
          <Link className="selection-chip" to={buildGalaxyUrl()}>
            Volver a Galaxia
          </Link>
        </div>
      </UiCard>

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Limites honestos</p>
            <h3>Lo que esta ruta si y no hace</h3>
          </div>
          <UiBadge>Lectura clara</UiBadge>
        </div>
        <ul className="stack-list compact-list">
          {limitations.map((item) => (
            <li key={item}>{item}</li>
          ))}
        </ul>
      </UiCard>
    </section>
  );
}

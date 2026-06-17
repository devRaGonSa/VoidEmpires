import { FormEvent, useState } from "react";
import { Link } from "react-router-dom";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { savePlayableSession } from "../utils/playableSession";
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
    value: "Registro y confirmacion existen en backend",
    detail: "Esta pantalla no crea credenciales ni usa el endpoint de registro.",
  },
  {
    label: "Sesion",
    value: "Memoria local de navegacion",
    detail: "Guarda ids devueltos por Development para reabrir cabinas; no es cookie, token ni rol.",
  },
  {
    label: "Propiedad",
    value: "Revalidada por cada lectura backend",
    detail: "Las cabinas usan civilizationId y planetId explicitos y vuelven a consultar el estado autoritativo.",
  },
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
      return "El backend no tiene persistencia configurada para crear el inicio jugable.";
    default:
      return message;
  }
}

export function OnboardingPage() {
  const [displayName, setDisplayName] = useState("");
  const [civilizationName, setCivilizationName] = useState("");
  const [homePlanetName, setHomePlanetName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [technicalDetail, setTechnicalDetail] = useState<string | null>(null);
  const [createdStart, setCreatedStart] = useState<CreatedPlayableStart | null>(null);
  const [limitations, setLimitations] = useState<string[]>([
    "Flujo solo para desarrollo.",
    "No crea una sesion autenticada.",
    "Las cabinas seguiran navegando con `civilizationId` y `planetId` en la URL.",
  ]);

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
        const backendError = result.response?.errors[0] ?? "El backend no pudo crear el inicio jugable.";
        setError(toSpanishOnboardingError(backendError));
        setTechnicalDetail(`Backend response: HTTP ${result.httpStatus}${result.bodyParseFailed ? " sin cuerpo JSON legible." : "."}`);
        return;
      }

      setLimitations(
        result.response.limitations.length > 0
          ? result.response.limitations.map(toSpanishOnboardingError)
          : limitations,
      );

      if (!result.response.civilizationId || !result.response.homePlanetId) {
        setError("El backend creo el inicio, pero no devolvio el contexto minimo para abrir Planeta.");
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
        setTechnicalDetail("El inicio fue creado, pero el navegador no permitio guardar la memoria local de navegacion.");
      }
    } catch (requestError) {
      setError("No se pudo contactar con el backend de Development para crear el inicio jugable.");
      setTechnicalDetail(requestError instanceof Error ? requestError.message : "Error desconocido.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Onboarding v1"
        title="Crear inicio jugable"
        description="Esta ruta crea un punto de partida Development-only y te lleva directamente a Planeta con el contexto devuelto por el backend."
        developmentNote="No es un login, no guarda credenciales y no resuelve una sesion persistente de produccion."
        badges={
          <>
            <UiBadge tone="warn">Development-only</UiBadge>
            <UiBadge>Backend autoritativo</UiBadge>
            <UiBadge>Sin autenticacion real</UiBadge>
          </>
        }
      />

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Cuenta y acceso</p>
            <h3>Frontera actual de autenticacion</h3>
          </div>
          <UiBadge tone="warn">Preparada, no productizada</UiBadge>
        </div>
        <p className="figma-panel-note">
          VoidEmpires ya tiene una base tecnica de cuenta con registro y confirmacion por email, pero este inicio jugable no resuelve una sesion autenticada ni permisos de produccion.
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

      <UiCard className="panel strategic-loader-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Nuevo inicio</p>
            <h3>Preparar comandante y colonia base</h3>
          </div>
          <UiBadge tone="warn">Sin login</UiBadge>
        </div>
        <p className="figma-panel-note">
          El backend crea un inicio aislado para desarrollo y devuelve los ids necesarios para abrir la cabina de Planeta sin tocar la civilizacion sembrada de validacion.
        </p>

        <form className="query-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Nombre del jugador</span>
            <input
              type="text"
              value={displayName}
              onChange={(event) => setDisplayName(event.target.value)}
              placeholder="Comandante Vega"
              maxLength={128}
            />
          </label>
          <label className="field">
            <span>Nombre de la civilizacion</span>
            <input
              type="text"
              value={civilizationName}
              onChange={(event) => setCivilizationName(event.target.value)}
              placeholder="Dominio Solar"
              maxLength={128}
            />
          </label>
          <label className="field">
            <span>Planeta inicial opcional</span>
            <input
              type="text"
              value={homePlanetName}
              onChange={(event) => setHomePlanetName(event.target.value)}
              placeholder="Nova Prime"
              maxLength={128}
            />
          </label>
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Creando inicio..." : "Crear inicio jugable"}
          </button>
        </form>

        {error ? <p className="error-text">{error}</p> : null}
        {technicalDetail ? <p className="figma-panel-note">{technicalDetail}</p> : null}
        {error ? (
          <p className="figma-panel-note">
            Arranca la API local y reintenta; si el backend responde con error, el detalle tecnico queda visible arriba.
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
              La colonia inicial ya existe en el backend. La memoria local solo conserva los enlaces de navegacion para este navegador.
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
            <details className="technical-disclosure">
              <summary>Detalles de navegacion local</summary>
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Jugador</span><strong>{createdStart.playerDisplayName}</strong></div>
                <div className="figma-data-row"><span>Civilizacion</span><strong>{createdStart.civilizationName}</strong></div>
                <div className="figma-data-row"><span>Planeta</span><strong>{createdStart.planetName}</strong></div>
                <div className="figma-data-row"><span>Memoria local</span><strong>{createdStart.storedLocally ? "Guardada" : "No disponible"}</strong></div>
                <div className="figma-data-row"><span>civilizationId</span><strong>{createdStart.civilizationId}</strong></div>
                <div className="figma-data-row"><span>planetId</span><strong>{createdStart.planetId}</strong></div>
              </div>
            </details>
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

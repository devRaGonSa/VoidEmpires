import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import type { AccountSessionResponse } from "../api/accountTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatAccountRequestException, formatLoginErrors } from "../utils/accountErrorCopy";
import { buildPlanetUrl, buildRegisterUrl } from "../utils/routeUrls";

function resolveNextRoute(session: AccountSessionResponse | null) {
  if (!session?.succeeded) {
    return "/";
  }

  return session.nextRoute
    ?? (session.civilizationId && session.homePlanetId
      ? buildPlanetUrl(session.civilizationId, session.homePlanetId)
      : "/");
}

export function LoginPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setErrors([]);

    try {
      const result = await voidEmpiresApi.account.login({
        email: email.trim(),
        password,
      });

      if (result.httpStatus !== 200 || !result.response?.succeeded) {
        setErrors(formatLoginErrors(result));
        return;
      }

      navigate(resolveNextRoute(result.response), { replace: true });
    } catch {
      setErrors(formatAccountRequestException());
    } finally {
      setPassword("");
      setIsSubmitting(false);
    }
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Cuenta"
        title="Entrar"
        description="Accede con tu correo para recuperar el comandante, la civilizacion y el planeta principal preparados por el servidor."
        developmentNote="La contrasena se envia solo al backend de cuentas y se borra del formulario al terminar."
        badges={
          <>
            <UiBadge tone="good">Cuenta existente</UiBadge>
            <UiBadge>Sesion segura</UiBadge>
          </>
        }
      />

      <UiCard className="panel strategic-loader-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Acceso</p>
            <h3>Correo y contrasena</h3>
          </div>
          <UiBadge tone="good">Entrar</UiBadge>
        </div>
        <form className="query-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Correo</span>
            <input type="email" value={email} onChange={(event) => setEmail(event.target.value)} />
          </label>
          <label className="field">
            <span>Contrasena</span>
            <input type="password" value={password} onChange={(event) => setPassword(event.target.value)} />
          </label>
          <button type="submit" disabled={isSubmitting}>{isSubmitting ? "Entrando..." : "Entrar"}</button>
        </form>
        {errors.length > 0 ? (
          <ul className="stack-list compact-list">
            {errors.map((error) => <li className="error-text" key={error}>{error}</li>)}
          </ul>
        ) : null}
        <div className="selection-chip-row">
          <Link className="selection-chip" to={buildRegisterUrl()}>Crear cuenta</Link>
        </div>
      </UiCard>
    </section>
  );
}

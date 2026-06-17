import { type KeyboardEvent, type ReactNode, useEffect, useId, useRef } from "react";
import { ActionStateBadge, type ActionState } from "./ActionStateBadge";
import { UiBadge } from "./ui/UiBadge";

interface GameModalAction {
  label: string;
  onClick: () => void;
  disabled?: boolean;
}

type GameModalScope = "gameplay" | "development";

interface GameModalProps {
  isOpen: boolean;
  title: string;
  description?: string;
  children?: ReactNode;
  primaryAction: GameModalAction;
  secondaryAction?: GameModalAction;
  actionScope?: GameModalScope;
  isBusy?: boolean;
  canClose?: boolean;
  closeLabel?: string;
  onClose?: () => void;
}

export function GameModal({
  isOpen,
  title,
  description,
  children,
  primaryAction,
  secondaryAction,
  actionScope,
  isBusy = false,
  canClose = true,
  closeLabel = "Cerrar",
  onClose,
}: GameModalProps) {
  const titleId = useId();
  const descriptionId = useId();
  const dialogRef = useRef<HTMLElement | null>(null);
  const closeAllowed = canClose && !isBusy;
  const primaryActionState: ActionState = isBusy ? "loading" : primaryAction.disabled ? "blocked" : "pending";
  const scopeBadge =
    actionScope === "development" ? (
      <ActionStateBadge state="developmentOnly" />
    ) : actionScope === "gameplay" ? (
      <UiBadge tone="warn">Confirmacion obligatoria</UiBadge>
    ) : null;

  useEffect(() => {
    if (isOpen) {
      dialogRef.current?.focus();
    }
  }, [isOpen]);

  if (!isOpen) {
    return null;
  }

  function handleClose() {
    if (!closeAllowed) {
      return;
    }

    onClose?.();
  }

  function handleKeyDown(event: KeyboardEvent<HTMLDivElement>) {
    if (event.key === "Escape") {
      event.preventDefault();
      handleClose();
    }
  }

  return (
    <div
      className="game-modal-backdrop"
      onClick={handleClose}
    >
      <section
        aria-describedby={description ? descriptionId : undefined}
        aria-labelledby={titleId}
        aria-modal="true"
        className="game-modal"
        onClick={(event) => event.stopPropagation()}
        onKeyDown={handleKeyDown}
        ref={dialogRef}
        role="dialog"
        tabIndex={-1}
      >
        <header className="game-modal-header">
          <div className="game-modal-copy">
            <p className="eyebrow">Confirmacion</p>
            <h3 id={titleId}>{title}</h3>
            {description ? (
              <p className="game-modal-description" id={descriptionId}>
                {description}
              </p>
            ) : null}
            <div className="figma-badge-row">
              {scopeBadge}
              <ActionStateBadge
                state={primaryActionState}
                label={isBusy ? "Procesando" : primaryAction.disabled ? "Bloqueado" : "Pendiente de confirmacion"}
              />
            </div>
          </div>
          <button
            aria-label={closeLabel}
            className="game-modal-close"
            disabled={!closeAllowed}
            onClick={handleClose}
            type="button"
          >
            {closeLabel}
          </button>
        </header>

        {children ? <div className="game-modal-body">{children}</div> : null}

        <footer className="game-modal-actions">
          {secondaryAction ? (
            <button
              className="planet-action-button-secondary"
              disabled={secondaryAction.disabled || isBusy}
              onClick={secondaryAction.onClick}
              type="button"
            >
              {secondaryAction.label}
            </button>
          ) : null}
          <button
            disabled={primaryAction.disabled || isBusy}
            aria-busy={isBusy ? "true" : undefined}
            onClick={primaryAction.onClick}
            type="button"
          >
            {isBusy ? "Procesando..." : primaryAction.label}
          </button>
        </footer>
      </section>
    </div>
  );
}

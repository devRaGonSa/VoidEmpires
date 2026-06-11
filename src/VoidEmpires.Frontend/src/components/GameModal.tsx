import { type KeyboardEvent, type ReactNode, useId } from "react";

interface GameModalAction {
  label: string;
  onClick: () => void;
  disabled?: boolean;
}

interface GameModalProps {
  isOpen: boolean;
  title: string;
  description?: string;
  children?: ReactNode;
  primaryAction: GameModalAction;
  secondaryAction?: GameModalAction;
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
  isBusy = false,
  canClose = true,
  closeLabel = "Cerrar",
  onClose,
}: GameModalProps) {
  const titleId = useId();
  const descriptionId = useId();
  const closeAllowed = canClose && !isBusy;

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

import type { HTMLAttributes, ReactNode } from "react";

interface UiCardProps extends HTMLAttributes<HTMLElement> {
  as?: "article" | "section" | "div";
  children: ReactNode;
}

export function UiCard({
  as = "article",
  children,
  className,
  ...rest
}: UiCardProps) {
  const Component = as;
  const resolvedClassName = className
    ? `ui-card ${className}`
    : "ui-card";

  return (
    <Component className={resolvedClassName} {...rest}>
      {children}
    </Component>
  );
}

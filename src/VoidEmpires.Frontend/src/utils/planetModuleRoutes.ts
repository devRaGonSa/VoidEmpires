import type { PlanetModule } from "../api/planetTypes";

export interface PlanetModuleRouteInfo {
  module: PlanetModule;
  path: string;
  label: string;
  title: string;
  purpose: string;
  belongsTo: string[];
  excludes: string[];
}

export const specializedPlanetModuleRoutes: readonly PlanetModuleRouteInfo[] = [
  {
    module: "Research",
    path: "/research",
    label: "Investigacion",
    title: "Cabina de Investigacion",
    purpose: "Preparada para la investigacion, la consulta de progreso y la futura gestion de tecnologia.",
    belongsTo: [
      "Laboratorios, proyectos y mejoras tecnologicas.",
      "Lectura del estado de investigacion activa.",
    ],
    excludes: [
      "Construccion general de edificios.",
      "Produccion de tropas o flotas.",
    ],
  },
  {
    module: "GroundArmy",
    path: "/ground-army",
    label: "Ejercito Tierra",
    title: "Cabina de Ejercito Tierra",
    purpose: "Preparada para la organizacion terrestre y el seguimiento del estado de fuerza.",
    belongsTo: [
      "Entrenamiento, despliegue y lectura de fuerzas terrestres.",
      "Estado y disponibilidad de estructuras de mando terrestre.",
    ],
    excludes: [
      "Investigacion de tecnologia.",
      "Produccion naval o defensa orbital.",
    ],
  },
  {
    module: "Shipyard",
    path: "/shipyard",
    label: "Astillero",
    title: "Cabina de Astillero",
    purpose: "Preparada para la produccion orbital, la lectura de taxonomia naval y el futuro seguimiento de stock y colas.",
    belongsTo: [
      "Catalogo orbital, etiquetas de nave y lectura de roles navales.",
      "Produccion orbital controlada, stock local y colas futuras.",
    ],
    excludes: [
      "Investigacion, gestion terrestre y defensa planetaria pura.",
      "Movimiento, division, fusion o ejecucion tactica de flotas.",
    ],
  },
  {
    module: "Defenses",
    path: "/defenses",
    label: "Defensas",
    title: "Cabina de Defensas",
    purpose: "Preparada para la lectura defensiva y la futura gestion de proteccion planetaria.",
    belongsTo: [
      "Estructuras defensivas, lectura de cobertura y proteccion del planeta.",
      "Estado de seguridad y amenazas futuras.",
    ],
    excludes: [
      "Construccion general de infraestructura.",
      "Produccion naval o terrestre.",
    ],
  },
] as const;

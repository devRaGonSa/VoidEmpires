namespace VoidEmpires.Application.Visuals;

public sealed record PlanetVisualProfileDto(
    string SurfaceProfile,
    string LightDistributionMode,
    string PlatformMode,
    string AtmosphereProfile,
    string CloudProfile,
    bool SupportsNightLights,
    bool SupportsSurfacePlatforms,
    bool SupportsOrbitalMegastructureHints);

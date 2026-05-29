namespace VoidEmpires.Application.Visuals;

public sealed record PlanetVisualIntensityResult(
    int VisualSeed,
    float ColonizationIntensity,
    float UrbanIntensity,
    float IndustrialIntensity,
    float TerraformingIntensity,
    float MilitaryIntensity,
    float OrbitalPresenceIntensity);

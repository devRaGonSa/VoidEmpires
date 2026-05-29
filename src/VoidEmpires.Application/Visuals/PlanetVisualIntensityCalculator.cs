namespace VoidEmpires.Application.Visuals;

public static class PlanetVisualIntensityCalculator
{
    public static PlanetVisualIntensityResult Calculate(PlanetVisualIntensityInput input)
    {
        if (input.PlanetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.", nameof(input));
        }

        if (input.Size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(input), "Planet size must be positive.");
        }

        var size = Math.Max(input.Size, 1);
        var ownershipFactor = input.IsOwned ? 0.05f : 0f;
        var colonizationStatusFactor = input.ColonizationStatus switch
        {
            Domain.Galaxy.PlanetColonizationStatus.Colonized => 0.10f,
            Domain.Galaxy.PlanetColonizationStatus.Reserved => 0.04f,
            Domain.Galaxy.PlanetColonizationStatus.Ruined => 0.01f,
            _ => 0f
        };

        var colonizationIntensity = Clamp01(
            ownershipFactor +
            colonizationStatusFactor +
            input.BuildingCount * 0.015f +
            input.TotalBuildingLevels / (float)size * 0.25f);

        var urbanIntensity = Clamp01(input.UrbanBuildingLevels / (float)size);
        var industrialIntensity = Clamp01(input.IndustrialBuildingLevels / (float)size);
        var terraformingIntensity = Clamp01(input.TerraformingBuildingLevels / (float)size);
        var militaryIntensity = Clamp01(input.MilitaryBuildingLevels / (float)size);
        var orbitalPresenceIntensity = Clamp01(input.OrbitalGroupStrength / ExpectedOrbitalPresenceCapacity(size));

        return new PlanetVisualIntensityResult(
            GetStableVisualSeed(input.PlanetId),
            colonizationIntensity,
            urbanIntensity,
            industrialIntensity,
            terraformingIntensity,
            militaryIntensity,
            orbitalPresenceIntensity);
    }

    private static float ExpectedOrbitalPresenceCapacity(int planetSize) => Math.Max(planetSize * 0.5f, 1f);

    private static int GetStableVisualSeed(Guid planetId)
    {
        var bytes = planetId.ToByteArray();
        var hash = 17;

        foreach (var value in bytes)
        {
            hash = unchecked(hash * 31 + value);
        }

        return hash == int.MinValue ? 0 : Math.Abs(hash);
    }

    private static float Clamp01(float value) => Math.Clamp(value, 0f, 1f);
}

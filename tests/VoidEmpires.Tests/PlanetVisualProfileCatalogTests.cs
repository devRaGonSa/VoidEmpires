using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Tests;

public class PlanetVisualProfileCatalogTests
{
    [Theory]
    [InlineData(PlanetType.Terran, "terran_continental", "land_city_networks", "surface_cities_and_minor_orbitals", true, true)]
    [InlineData(PlanetType.Desert, "desert_dune_basins", "oasis_basin_clusters", "solar_domes_and_convoy_routes", true, true)]
    [InlineData(PlanetType.Ice, "ice_cracked", "isolated_colony_nodes", "thermal_domes_and_surface_platforms", true, true)]
    [InlineData(PlanetType.Volcanic, "volcanic_basalt_lava", "shielded_platform_clusters", "thermal_shields_and_extractor_platforms", true, true)]
    [InlineData(PlanetType.Oceanic, "oceanic_deep_water", "floating_arcologies", "floating_cities_and_marine_routes", true, true)]
    [InlineData(PlanetType.Barren, "barren_cratered_regolith", "mining_outposts", "mines_extractors_and_cargo_ports", true, true)]
    [InlineData(PlanetType.GasGiant, "gas_giant_banded_atmosphere", "orbital_only", "orbital_stations_and_high_atmosphere_platforms", false, false)]
    public void GetProfileReturnsDistinctProfileForPlanetType(
        PlanetType planetType,
        string expectedSurfaceProfile,
        string expectedLightDistributionMode,
        string expectedPlatformMode,
        bool expectedNightLights,
        bool expectedSurfacePlatforms)
    {
        var profile = PlanetVisualProfileCatalog.GetProfile(planetType);

        Assert.Equal(expectedSurfaceProfile, profile.SurfaceProfile);
        Assert.Equal(expectedLightDistributionMode, profile.LightDistributionMode);
        Assert.Equal(expectedPlatformMode, profile.PlatformMode);
        Assert.Equal(expectedNightLights, profile.SupportsNightLights);
        Assert.Equal(expectedSurfacePlatforms, profile.SupportsSurfacePlatforms);
    }

    [Fact]
    public void GasGiantProfileSupportsOrbitalHintsWithoutSurfaceLightsOrPlatforms()
    {
        var profile = PlanetVisualProfileCatalog.GetProfile(PlanetType.GasGiant);

        Assert.False(profile.SupportsNightLights);
        Assert.False(profile.SupportsSurfacePlatforms);
        Assert.True(profile.SupportsOrbitalMegastructureHints);
        Assert.Equal("orbital_only", profile.LightDistributionMode);
    }
}

using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.Visuals;

public static class PlanetVisualProfileCatalog
{
    public static PlanetVisualProfileDto GetProfile(PlanetType planetType) => planetType switch
    {
        PlanetType.Terran => new PlanetVisualProfileDto(
            "terran_continental",
            "land_city_networks",
            "surface_cities_and_minor_orbitals",
            "blue_fresnel",
            "temperate_clouds",
            SupportsNightLights: true,
            SupportsSurfacePlatforms: true,
            SupportsOrbitalMegastructureHints: true),

        PlanetType.Desert => new PlanetVisualProfileDto(
            "desert_dune_basins",
            "oasis_basin_clusters",
            "solar_domes_and_convoy_routes",
            "thin_warm_haze",
            "dust_streams",
            SupportsNightLights: true,
            SupportsSurfacePlatforms: true,
            SupportsOrbitalMegastructureHints: true),

        PlanetType.Ice => new PlanetVisualProfileDto(
            "ice_cracked",
            "isolated_colony_nodes",
            "thermal_domes_and_surface_platforms",
            "thin_blue_frost",
            "polar_mist",
            SupportsNightLights: true,
            SupportsSurfacePlatforms: true,
            SupportsOrbitalMegastructureHints: true),

        PlanetType.Volcanic => new PlanetVisualProfileDto(
            "volcanic_basalt_lava",
            "shielded_platform_clusters",
            "thermal_shields_and_extractor_platforms",
            "hot_ash_haze",
            "ash_clouds",
            SupportsNightLights: true,
            SupportsSurfacePlatforms: true,
            SupportsOrbitalMegastructureHints: true),

        PlanetType.Oceanic => new PlanetVisualProfileDto(
            "oceanic_deep_water",
            "floating_arcologies",
            "floating_cities_and_marine_routes",
            "humid_blue_glow",
            "storm_bands",
            SupportsNightLights: true,
            SupportsSurfacePlatforms: true,
            SupportsOrbitalMegastructureHints: true),

        PlanetType.Barren => new PlanetVisualProfileDto(
            "barren_cratered_regolith",
            "mining_outposts",
            "mines_extractors_and_cargo_ports",
            "thin_gray_haze",
            "dust_veil",
            SupportsNightLights: true,
            SupportsSurfacePlatforms: true,
            SupportsOrbitalMegastructureHints: true),

        PlanetType.GasGiant => new PlanetVisualProfileDto(
            "gas_giant_banded_atmosphere",
            "orbital_only",
            "orbital_stations_and_high_atmosphere_platforms",
            "deep_gas_fresnel",
            "storm_belts",
            SupportsNightLights: false,
            SupportsSurfacePlatforms: false,
            SupportsOrbitalMegastructureHints: true),

        _ => throw new ArgumentOutOfRangeException(nameof(planetType), planetType, "Unsupported planet type.")
    };
}

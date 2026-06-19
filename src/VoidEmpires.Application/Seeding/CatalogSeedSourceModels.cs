namespace VoidEmpires.Application.Seeding;

public sealed record CatalogSeedCostDto(
    decimal Credits,
    decimal Metal,
    decimal Crystal,
    decimal Gas);

public sealed record CatalogSeedSourceEntry(
    string Key,
    string DisplayName,
    string Description,
    string CategoryKey,
    string CategoryLabel,
    string? RoleKey,
    string? RoleLabel,
    string? ModuleKey,
    string? ModuleLabel,
    string? ImageKey,
    string? IconKey,
    int SortOrder,
    IReadOnlyList<string> Tags,
    CatalogSeedCostDto? Cost = null,
    decimal? DurationMinutes = null);

namespace VoidEmpires.Domain.Economy;

public sealed record ResourceCatalogEntry(
    string Key,
    string DisplayName,
    string Description,
    bool IsPersisted,
    bool IsSpendable,
    string ClassKey,
    string ClassLabel,
    string ImageKey,
    string IconKey,
    int SortOrder,
    IReadOnlyList<string> Tags);

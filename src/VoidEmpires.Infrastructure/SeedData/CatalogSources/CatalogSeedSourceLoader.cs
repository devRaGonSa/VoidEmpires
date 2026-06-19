using System.Text.Json;
using System.Text.RegularExpressions;
using VoidEmpires.Application.Seeding;

namespace VoidEmpires.Infrastructure.SeedData.CatalogSources;

public sealed class CatalogSeedSourceLoader
{
    private static readonly Regex AssetKeyPattern = new("^[a-z0-9]+([.-][a-z0-9]+)*$", RegexOptions.Compiled);

    public IReadOnlyList<CatalogSeedSourceEntry> LoadFile(string path) =>
        LoadJson(File.ReadAllText(path), path);

    public IReadOnlyList<CatalogSeedSourceEntry> LoadJson(string json, string sourceName)
    {
        var entries = JsonSerializer.Deserialize<List<CatalogSeedSourceEntry>>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web))
            ?? throw new InvalidOperationException($"Catalog source '{sourceName}' could not be deserialized.");
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var label = $"Catalog source '{sourceName}' row {i + 1}";

            Require(entry.Key, label, nameof(entry.Key));
            Require(entry.DisplayName, label, nameof(entry.DisplayName));
            Require(entry.Description, label, nameof(entry.Description));
            Require(entry.CategoryKey, label, nameof(entry.CategoryKey));
            Require(entry.CategoryLabel, label, nameof(entry.CategoryLabel));
            if (entry.SortOrder <= 0) throw new InvalidOperationException($"{label} has invalid SortOrder '{entry.SortOrder}'.");
            if (!seenKeys.Add(entry.Key)) throw new InvalidOperationException($"Catalog source '{sourceName}' contains duplicate key '{entry.Key}'.");
            if (string.IsNullOrWhiteSpace(entry.ImageKey) && string.IsNullOrWhiteSpace(entry.IconKey)) throw new InvalidOperationException($"{label} must provide ImageKey or IconKey.");
            if (!string.IsNullOrWhiteSpace(entry.ImageKey) && !AssetKeyPattern.IsMatch(entry.ImageKey)) throw new InvalidOperationException($"{label} has invalid ImageKey '{entry.ImageKey}'.");
            if (!string.IsNullOrWhiteSpace(entry.IconKey) && !AssetKeyPattern.IsMatch(entry.IconKey)) throw new InvalidOperationException($"{label} has invalid IconKey '{entry.IconKey}'.");
            if (entry.Cost is not null && (entry.Cost.Credits < 0 || entry.Cost.Metal < 0 || entry.Cost.Crystal < 0 || entry.Cost.Gas < 0)) throw new InvalidOperationException($"{label} has negative cost values.");
            if (entry.DurationMinutes is < 0) throw new InvalidOperationException($"{label} has invalid DurationMinutes '{entry.DurationMinutes}'.");
        }

        return entries;
    }

    private static void Require(string? value, string label, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{label} is missing required field '{propertyName}'.");
        }
    }
}

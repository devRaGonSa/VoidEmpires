using VoidEmpires.Application.Seeding;

namespace VoidEmpires.Infrastructure.SeedData.CatalogSources;

public sealed class FinalCatalogSeedService(CatalogSeedSourceLoader loader) : IFinalCatalogSeedService
{
    private static readonly string[] CatalogFiles =
    [
        "buildings.catalog.json",
        "research.catalog.json",
        "orbital-assets.catalog.json",
        "defenses.catalog.json",
        "resources.catalog.json"
    ];

    public Task<FinalCatalogSeedResult> RunAsync(FinalCatalogSeedRequest request, CancellationToken cancellationToken = default)
    {
        var sourceDirectory = string.IsNullOrWhiteSpace(request.SourceDirectory)
            ? Path.Combine(AppContext.BaseDirectory, "SeedData", "CatalogSources")
            : request.SourceDirectory.Trim();

        if (!Directory.Exists(sourceDirectory))
        {
            return Task.FromResult(new FinalCatalogSeedResult(false, request.DryRun, true, [], [], [$"Catalog seed source directory '{sourceDirectory}' was not found."]));
        }

        var summaries = new List<FinalCatalogSeedCatalogSummary>(CatalogFiles.Length);
        foreach (var fileName in CatalogFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var path = Path.Combine(sourceDirectory, fileName);
            if (!File.Exists(path))
            {
                return Task.FromResult(new FinalCatalogSeedResult(false, request.DryRun, true, summaries, [], [$"Catalog seed source file '{fileName}' was not found in '{sourceDirectory}'."]));
            }

            summaries.Add(new FinalCatalogSeedCatalogSummary(fileName, loader.LoadFile(path).Count));
        }

        if (!request.DryRun)
        {
            return Task.FromResult(new FinalCatalogSeedResult(
                false,
                false,
                true,
                summaries,
                ["Catalog source loading succeeded."],
                ["DryRun=false is not supported yet because final relational catalog tables and manual apply wiring are still deferred."]));
        }

        return Task.FromResult(new FinalCatalogSeedResult(
            true,
            true,
            true,
            summaries,
            ["Catalog source loading and validation succeeded."],
            []));
    }
}

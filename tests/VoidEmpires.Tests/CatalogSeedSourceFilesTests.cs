using System.Text.Json;
using VoidEmpires.Infrastructure.SeedData.CatalogSources;

namespace VoidEmpires.Tests;

public class CatalogSeedSourceFilesTests
{
    private readonly CatalogSeedSourceLoader loader = new();

    [Theory]
    [InlineData("buildings.catalog.json", 19)]
    [InlineData("research.catalog.json", 8)]
    [InlineData("orbital-assets.catalog.json", 4)]
    [InlineData("defenses.catalog.json", 5)]
    [InlineData("resources.catalog.json", 7)]
    public void CatalogSeedSourceFileExistsAndContainsExpectedRows(string fileName, int expectedCount)
    {
        var repositoryRoot = FindRepositoryRoot();
        var filePath = Path.Combine(
            repositoryRoot,
            "src",
            "VoidEmpires.Infrastructure",
            "SeedData",
            "CatalogSources",
            fileName);

        Assert.True(File.Exists(filePath), $"Expected catalog seed source file '{fileName}' to exist in the repository catalog-source folder.");

        var rows = loader.LoadFile(filePath);

        Assert.Equal(expectedCount, rows.Count);
        Assert.All(rows, row =>
        {
            Assert.False(string.IsNullOrWhiteSpace(row.Key));
            Assert.False(string.IsNullOrWhiteSpace(row.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(row.Description));
            Assert.True(!string.IsNullOrWhiteSpace(row.ImageKey) || !string.IsNullOrWhiteSpace(row.IconKey));
        });
    }

    [Fact]
    public void CatalogSeedSourceLoaderRejectsDuplicateKeys()
    {
        const string json = """
            [{ "key":"Alpha","displayName":"Uno","description":"Desc","categoryKey":"cat","categoryLabel":"Cat","imageKey":"asset.one","iconKey":"icon.one","sortOrder":1,"tags":["ok"] },
             { "key":"Alpha","displayName":"Dos","description":"Desc","categoryKey":"cat","categoryLabel":"Cat","imageKey":"asset.two","iconKey":"icon.two","sortOrder":2,"tags":["ok"] }]
            """;

        var error = Assert.Throws<InvalidOperationException>(() => loader.LoadJson(json, "duplicate-test"));

        Assert.Contains("duplicate key 'Alpha'", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CatalogSeedSourceLoaderRejectsMissingRequiredMetadata()
    {
        const string json = """[{ "key":"","displayName":"Uno","description":"Desc","categoryKey":"cat","categoryLabel":"Cat","imageKey":"asset.one","iconKey":"icon.one","sortOrder":1,"tags":["ok"] }]""";

        var error = Assert.Throws<InvalidOperationException>(() => loader.LoadJson(json, "required-test"));

        Assert.Contains("missing required field 'Key'", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CatalogSeedSourceLoaderRejectsInvalidAssetKeyShape()
    {
        const string json = """[{ "key":"Alpha","displayName":"Uno","description":"Desc","categoryKey":"cat","categoryLabel":"Cat","imageKey":"BAD KEY","iconKey":"icon.one","sortOrder":1,"tags":["ok"] }]""";

        var error = Assert.Throws<InvalidOperationException>(() => loader.LoadJson(json, "asset-key-test"));

        Assert.Contains("invalid ImageKey", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CatalogSeedSourceLoaderRejectsInvalidSortOrderCostAndDuration()
    {
        const string json = """[{ "key":"Alpha","displayName":"Uno","description":"Desc","categoryKey":"cat","categoryLabel":"Cat","imageKey":"asset.one","iconKey":"icon.one","sortOrder":0,"tags":["ok"] }]""";

        var error = Assert.Throws<InvalidOperationException>(() => loader.LoadJson(json, "numeric-test"));

        Assert.Contains("invalid SortOrder", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CatalogSeedSourceLoaderRejectsNegativeCostAndDuration()
    {
        const string costJson = """[{ "key":"Alpha","displayName":"Uno","description":"Desc","categoryKey":"cat","categoryLabel":"Cat","imageKey":"asset.one","iconKey":"icon.one","sortOrder":1,"tags":["ok"],"cost":{"credits":-1,"metal":0,"crystal":0,"gas":0} }]""";
        const string durationJson = """[{ "key":"Alpha","displayName":"Uno","description":"Desc","categoryKey":"cat","categoryLabel":"Cat","imageKey":"asset.one","iconKey":"icon.one","sortOrder":1,"tags":["ok"],"durationMinutes":-5 }]""";

        var costError = Assert.Throws<InvalidOperationException>(() => loader.LoadJson(costJson, "cost-test"));
        var durationError = Assert.Throws<InvalidOperationException>(() => loader.LoadJson(durationJson, "duration-test"));

        Assert.Contains("negative cost values", costError.Message, StringComparison.Ordinal);
        Assert.Contains("invalid DurationMinutes", durationError.Message, StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "VoidEmpires.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the test output directory.");
    }
}

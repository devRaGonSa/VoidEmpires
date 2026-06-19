using System.Text.Json;

namespace VoidEmpires.Tests;

public class CatalogSeedSourceFilesTests
{
    [Theory]
    [InlineData("buildings.catalog.json", 15)]
    [InlineData("research.catalog.json", 8)]
    [InlineData("orbital-assets.catalog.json", 4)]
    [InlineData("defenses.catalog.json", 1)]
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

        using var document = JsonDocument.Parse(File.ReadAllText(filePath));
        var root = document.RootElement;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(expectedCount, root.GetArrayLength());

        foreach (var row in root.EnumerateArray())
        {
            Assert.Equal(JsonValueKind.Object, row.ValueKind);
            Assert.False(string.IsNullOrWhiteSpace(row.GetProperty("key").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(row.GetProperty("displayName").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(row.GetProperty("description").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(row.GetProperty("imageKey").GetString()));
        }
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

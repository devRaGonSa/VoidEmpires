using VoidEmpires.Application.Galaxy;

namespace VoidEmpires.Tests;

public class GalaxyGenerationContractTests
{
    [Fact]
    public void GenerateAndPersistRequestDefaultsToNoOverwrite()
    {
        var request = new GenerateAndPersistGalaxyRequest("Void Prime", "alpha-001", 25, 2, 6);

        Assert.False(request.OverwriteExisting);
    }

    [Fact]
    public void GenerateAndPersistSuccessCapturesDeterministicSummary()
    {
        var galaxyId = Guid.Parse("84e65c70-50ce-4d52-b59f-aa7594cc250d");

        var result = GenerateAndPersistGalaxyResult.Success(galaxyId, "Void Prime", 25, 104);

        Assert.True(result.Succeeded);
        Assert.Equal(galaxyId, result.GalaxyId);
        Assert.Equal("Void Prime", result.GalaxyName);
        Assert.Equal(25, result.SolarSystemCount);
        Assert.Equal(104, result.PlanetCount);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void GenerateAndPersistFailureCapturesErrorsWithoutSummary()
    {
        var result = GenerateAndPersistGalaxyResult.Failure("Galaxy already exists.");

        Assert.False(result.Succeeded);
        Assert.Null(result.GalaxyId);
        Assert.Null(result.GalaxyName);
        Assert.Equal(0, result.SolarSystemCount);
        Assert.Equal(0, result.PlanetCount);
        Assert.Equal(["Galaxy already exists."], result.Errors);
    }
}

using VoidEmpires.Domain.Colonization;

namespace VoidEmpires.Tests;

public class PlanetOwnershipDomainTests
{
    [Fact]
    public void CreateRejectsEmptyPlanetId()
    {
        Assert.Throws<ArgumentException>(() => PlanetOwnership.Create(Guid.Empty, Guid.NewGuid()));
    }

    [Fact]
    public void CreateRejectsEmptyCivilizationId()
    {
        Assert.Throws<ArgumentException>(() => PlanetOwnership.Create(Guid.NewGuid(), Guid.Empty));
    }

    [Fact]
    public void CreateSetsOwnershipValues()
    {
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();

        var ownership = PlanetOwnership.Create(planetId, civilizationId);

        Assert.Equal(planetId, ownership.PlanetId);
        Assert.Equal(civilizationId, ownership.CivilizationId);
        Assert.Equal(PlanetControlStatus.Active, ownership.Status);
    }
}

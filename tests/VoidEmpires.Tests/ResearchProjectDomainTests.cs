using VoidEmpires.Domain.Research;

namespace VoidEmpires.Tests;

public class ResearchProjectDomainTests
{
    [Fact]
    public void CreateStartsAtLevelOne()
    {
        var project = ResearchProject.Create(
            Guid.NewGuid(),
            ResearchType.ResourceExtraction);

        Assert.Equal(1, project.Level);
    }

    [Fact]
    public void UpgradeIncreasesLevel()
    {
        var project = ResearchProject.Create(
            Guid.NewGuid(),
            ResearchType.ResourceExtraction);

        project.Upgrade();

        Assert.Equal(2, project.Level);
    }
}

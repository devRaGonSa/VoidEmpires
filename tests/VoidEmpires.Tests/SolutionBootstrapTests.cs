using VoidEmpires.Application;
using VoidEmpires.Domain;
using VoidEmpires.Infrastructure;

namespace VoidEmpires.Tests;

public class SolutionBootstrapTests
{
    [Fact]
    public void AssemblyMarkersExposeExpectedProjectBoundaries()
    {
        Assert.Equal("VoidEmpires.Application", typeof(ApplicationAssemblyMarker).Assembly.GetName().Name);
        Assert.Equal("VoidEmpires.Domain", typeof(DomainAssemblyMarker).Assembly.GetName().Name);
        Assert.Equal("VoidEmpires.Infrastructure", typeof(InfrastructureAssemblyMarker).Assembly.GetName().Name);
    }
}

using VoidEmpires.Infrastructure.Fleets;

namespace VoidEmpires.Tests;

public class DevFleetActionManifestServiceTests
{
    [Fact]
    public void GetReturnsExpectedFleetActions()
    {
        var result = new DevFleetActionManifestService().Get();

        var keys = result.Actions.Select(x => x.ActionKey).ToArray();
        Assert.Contains("fleet.overview.read", keys);
        Assert.Contains("fleet.uiState.read", keys);
        Assert.Contains("fleet.travel.estimate", keys);
        Assert.Contains("fleet.transfer.create", keys);
        Assert.Contains("fleet.transfer.cancel", keys);
        Assert.Contains("fleet.transfer.complete", keys);
        Assert.Contains("fleet.group.split", keys);
        Assert.Contains("fleet.group.merge", keys);
    }

    [Fact]
    public void GetReturnsMachineReadableRouteAndFieldMetadata()
    {
        var result = new DevFleetActionManifestService().Get();

        var createTransfer = result.Actions.Single(x => x.ActionKey == "fleet.transfer.create");
        Assert.Equal("POST", createTransfer.Method);
        Assert.Equal("/api/dev/fleets/orbital-transfers/create", createTransfer.Route);
        Assert.False(createTransfer.IsReadOnly);
        Assert.Equal(201, createTransfer.SuccessStatus);
        Assert.Contains(createTransfer.RequiredFields, x => x.Name == "civilizationId" && x.IsRequired);
        Assert.Contains(createTransfer.RequiredFields, x => x.Name == "requestedAtUtc" && x.IsRequired);
        Assert.Contains(409, createTransfer.ErrorStatuses);

        var uiState = result.Actions.Single(x => x.ActionKey == "fleet.uiState.read");
        Assert.Equal("GET", uiState.Method);
        Assert.True(uiState.IsReadOnly);
        Assert.Equal(200, uiState.SuccessStatus);

        var estimate = result.Actions.Single(x => x.ActionKey == "fleet.travel.estimate");
        Assert.True(estimate.IsReadOnly);
        Assert.Contains(estimate.RequiredFields, x => x.Name == "destinationPlanetId" && x.IsRequired);
        Assert.Contains(409, estimate.ErrorStatuses);
        Assert.Contains("route profile", estimate.Notes, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("fuel readiness", estimate.Notes, StringComparison.OrdinalIgnoreCase);
    }
}

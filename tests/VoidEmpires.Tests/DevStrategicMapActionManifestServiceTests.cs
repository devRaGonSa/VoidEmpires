using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class DevStrategicMapActionManifestServiceTests
{
    [Fact]
    public void GetReturnsExpectedStrategicMapActions()
    {
        var result = new DevStrategicMapActionManifestService().Get();

        var keys = result.Actions.Select(x => x.ActionKey).ToArray();
        Assert.Equal([
            "strategicMap.read",
            "strategicMap.explorationPreview.read",
            "visual.system.read",
            "visual.planet.read",
            "fleet.uiState.read",
            "fleet.actionManifest.read",
            "strategicMap.actionManifest.read"
        ], keys);
    }

    [Fact]
    public void GetReturnsMachineReadableRouteAndFieldMetadata()
    {
        var result = new DevStrategicMapActionManifestService().Get();

        foreach (var action in result.Actions)
        {
            Assert.Equal("GET", action.Method);
            Assert.StartsWith("/api/dev/", action.Route, StringComparison.Ordinal);
            Assert.True(action.IsReadOnly);
            Assert.Equal(200, action.SuccessStatus);
            Assert.NotEmpty(action.ErrorStatuses);
            Assert.False(string.IsNullOrWhiteSpace(action.Notes));
        }

        var map = result.Actions.Single(x => x.ActionKey == "strategicMap.read");
        Assert.Contains(map.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);
        Assert.Contains("fleet presence", map.Notes, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("route/fuel", map.Notes, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("command availability", map.Notes, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("exploration preview", map.Notes, StringComparison.OrdinalIgnoreCase);

        var exploration = result.Actions.Single(x => x.ActionKey == "strategicMap.explorationPreview.read");
        Assert.Equal("/api/dev/strategic-map/exploration-preview", exploration.Route);
        Assert.Contains(exploration.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);
        Assert.Contains("read-only exploration readiness", exploration.Notes, StringComparison.OrdinalIgnoreCase);

        var manifest = result.Actions.Single(x => x.ActionKey == "strategicMap.actionManifest.read");
        Assert.Empty(manifest.RequiredFields);
    }
}

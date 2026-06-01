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
            "exploration.preview.read",
            "exploration.mission.create",
            "exploration.mission.completeDue",
            "exploration.knowledge.read",
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
            Assert.StartsWith("/api/dev/", action.Route, StringComparison.Ordinal);
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

        var explorationPreview = result.Actions.Single(x => x.ActionKey == "exploration.preview.read");
        Assert.True(explorationPreview.IsReadOnly);
        Assert.Equal("GET", explorationPreview.Method);
        Assert.Equal("/api/dev/strategic-map/exploration-preview", explorationPreview.Route);
        Assert.Contains(explorationPreview.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);

        var create = result.Actions.Single(x => x.ActionKey == "exploration.mission.create");
        Assert.Equal("POST", create.Method);
        Assert.False(create.IsReadOnly);
        Assert.Equal(201, create.SuccessStatus);
        Assert.Contains(409, create.ErrorStatuses);
        Assert.Contains(create.RequiredFields, x => x.Name == "targetPlanetId" && !x.IsRequired);
        Assert.Contains("does not complete missions", create.Notes, StringComparison.OrdinalIgnoreCase);

        var completeDue = result.Actions.Single(x => x.ActionKey == "exploration.mission.completeDue");
        Assert.Equal("POST", completeDue.Method);
        Assert.False(completeDue.IsReadOnly);
        Assert.Equal(200, completeDue.SuccessStatus);
        Assert.Contains(completeDue.RequiredFields, x => x.Name == "nowUtc" && x.Type == "DateTime" && x.IsRequired);
        Assert.Contains("Development operation", completeDue.Notes, StringComparison.OrdinalIgnoreCase);

        var knowledge = result.Actions.Single(x => x.ActionKey == "exploration.knowledge.read");
        Assert.True(knowledge.IsReadOnly);
        Assert.Equal("GET", knowledge.Method);
        Assert.Equal("/api/dev/strategic-map/exploration-knowledge", knowledge.Route);
        Assert.Contains(knowledge.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);
        Assert.Contains("ids-only", knowledge.Notes, StringComparison.OrdinalIgnoreCase);

        var manifest = result.Actions.Single(x => x.ActionKey == "strategicMap.actionManifest.read");
        Assert.Empty(manifest.RequiredFields);
    }
}

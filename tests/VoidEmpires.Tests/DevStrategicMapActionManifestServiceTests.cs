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
            "exploration.mission.list",
            "exploration.knowledge.read",
            "sensor.profile.read",
            "detection.coverage.read",
            "interception.opportunity.read",
            "diplomacy.contact.read",
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
        Assert.Contains("diplomacy contact", map.Notes, StringComparison.OrdinalIgnoreCase);

        var exploration = result.Actions.Single(x => x.ActionKey == "strategicMap.explorationPreview.read");
        Assert.Equal("/api/dev/strategic-map/exploration-preview", exploration.Route);
        Assert.Contains(exploration.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);
        Assert.Contains("read-only exploration readiness", exploration.Notes, StringComparison.OrdinalIgnoreCase);

        var create = result.Actions.Single(x => x.ActionKey == "exploration.mission.create");
        Assert.Equal("POST", create.Method);
        Assert.False(create.IsReadOnly);
        Assert.Equal(201, create.SuccessStatus);
        Assert.Contains(409, create.ErrorStatuses);
        Assert.Contains(create.RequiredFields, x => x.Name == "targetPlanetId" && !x.IsRequired);

        var knowledge = result.Actions.Single(x => x.ActionKey == "exploration.knowledge.read");
        Assert.True(knowledge.IsReadOnly);
        Assert.Equal("GET", knowledge.Method);
        Assert.Equal("/api/dev/strategic-map/exploration-knowledge", knowledge.Route);
        Assert.Contains(knowledge.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);

        var detectionCoverage = result.Actions.Single(x => x.ActionKey == "detection.coverage.read");
        Assert.True(detectionCoverage.IsReadOnly);
        Assert.Equal("GET", detectionCoverage.Method);
        Assert.Equal("/api/dev/strategic-map/detection-coverage", detectionCoverage.Route);
        Assert.Contains(detectionCoverage.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);

        var interception = result.Actions.Single(x => x.ActionKey == "interception.opportunity.read");
        Assert.True(interception.IsReadOnly);
        Assert.Equal("GET", interception.Method);
        Assert.Equal("/api/dev/strategic-map/interception-opportunities", interception.Route);
        Assert.Contains(interception.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);
        Assert.Contains("does not execute interception", interception.Notes, StringComparison.OrdinalIgnoreCase);

        var diplomacy = result.Actions.Single(x => x.ActionKey == "diplomacy.contact.read");
        Assert.True(diplomacy.IsReadOnly);
        Assert.Equal("GET", diplomacy.Method);
        Assert.Equal("/api/dev/strategic-map/diplomatic-contacts", diplomacy.Route);
        Assert.Contains(diplomacy.RequiredFields, x => x.Name == "civilizationId" && x.Type == "Guid" && x.IsRequired);
        Assert.Contains("does not create alliances", diplomacy.Notes, StringComparison.OrdinalIgnoreCase);

        var manifest = result.Actions.Single(x => x.ActionKey == "strategicMap.actionManifest.read");
        Assert.Empty(manifest.RequiredFields);
    }
}

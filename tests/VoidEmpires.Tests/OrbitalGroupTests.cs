using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class OrbitalGroupTests
{
    [Fact]
    public void CreateStationedStoresOriginAndCurrentPlanetSeparately()
    {
        var civilizationId = Guid.NewGuid();
        var originPlanetId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();

        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.EscortCraft,
            3);

        Assert.Equal(civilizationId, group.CivilizationId);
        Assert.Equal(originPlanetId, group.OriginPlanetId);
        Assert.Equal(currentPlanetId, group.CurrentPlanetId);
        Assert.Equal(SpaceAssetType.EscortCraft, group.AssetType);
        Assert.Equal(3, group.Quantity);
        Assert.Equal(OrbitalGroupStatus.Stationed, group.Status);
        Assert.True(group.IsStationedAwayFromOrigin);
    }

    [Fact]
    public void CreateStationedOnOriginPlanetIsNotAwayFromOrigin()
    {
        var planetId = Guid.NewGuid();

        var group = OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            planetId,
            planetId,
            SpaceAssetType.ScoutCraft,
            1);

        Assert.False(group.IsStationedAwayFromOrigin);
    }

    [Fact]
    public void ReserveChangesStationedGroupStatus()
    {
        var group = OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.CargoCraft,
            2);

        group.Reserve();

        Assert.Equal(OrbitalGroupStatus.Reserved, group.Status);
    }

    [Fact]
    public void ArriveAtMovesReservedGroupToDestinationAndStationed()
    {
        var originPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            originPlanetId,
            originPlanetId,
            SpaceAssetType.ScoutCraft,
            1);
        group.Reserve();

        group.ArriveAt(destinationPlanetId);

        Assert.Equal(destinationPlanetId, group.CurrentPlanetId);
        Assert.Equal(OrbitalGroupStatus.Stationed, group.Status);
        Assert.True(group.IsStationedAwayFromOrigin);
    }

    [Fact]
    public void SplitOffRejectsReservedGroup()
    {
        var group = OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.EscortCraft,
            3);
        group.Reserve();

        var exception = Assert.Throws<InvalidOperationException>(() => group.SplitOff(1));

        Assert.Equal("Only stationed orbital groups can be split.", exception.Message);
    }
}

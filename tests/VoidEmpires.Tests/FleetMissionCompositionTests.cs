using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class FleetMissionCompositionTests
{
    private static readonly DateTime Now = new(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void MixedCompositionMergesDuplicatesAndVersionsEachChange()
    {
        var mission = Create();
        mission.AddShips(SpaceAssetType.ScoutCraft, 6);
        Assert.Equal(1, mission.StateVersion);
        var scout = Assert.Single(mission.Ships);
        mission.AddShips(SpaceAssetType.ScoutCraft, 4);
        Assert.Same(scout, Assert.Single(mission.Ships));
        Assert.Equal((10, 2), (scout.Quantity, mission.StateVersion));
        mission.AddShips(SpaceAssetType.EscortCraft, 5);
        Assert.Equal(3, mission.StateVersion);
        mission.AddShips(SpaceAssetType.CargoCraft, 2);
        Assert.Equal((FleetMissionStatus.Preparing, 4), (mission.Status, mission.StateVersion));
        Assert.Equal(new[] { (SpaceAssetType.ScoutCraft, 10), (SpaceAssetType.CargoCraft, 2), (SpaceAssetType.EscortCraft, 5) }, Composition(mission));
        Assert.All(mission.Ships, ship => Assert.Equal(mission.Id, ship.MissionId));
    }

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft)]
    [InlineData(SpaceAssetType.CargoCraft)]
    [InlineData(SpaceAssetType.EscortCraft)]
    [InlineData(SpaceAssetType.ColonyCraft)]
    public void EveryKnownTypeIsAccepted(SpaceAssetType type)
    {
        var mission = Create();
        mission.AddShips(type, 1);
        Assert.Equal((mission.Id, type, 1), (Assert.Single(mission.Ships).MissionId, mission.Ships.Single().AssetType, mission.Ships.Single().Quantity));
    }

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft, 0)]
    [InlineData(SpaceAssetType.ScoutCraft, -1)]
    [InlineData(SpaceAssetType.CargoCraft, 0)]
    [InlineData((SpaceAssetType)0, 1)]
    [InlineData((SpaceAssetType)5, 1)]
    public void InvalidValuesLeaveCompositionAndVersionUntouched(SpaceAssetType type, int quantity)
    {
        var mission = Create();
        mission.AddShips(SpaceAssetType.ScoutCraft, 6);
        var before = Composition(mission);
        Assert.Throws<ArgumentOutOfRangeException>(() => mission.AddShips(type, quantity));
        Assert.Equal(before, Composition(mission));
        Assert.Equal((FleetMissionStatus.Preparing, 1), (mission.Status, mission.StateVersion));
    }

    [Fact]
    public void QuantityOverflowDoesNotPartiallyMutateTheAggregate()
    {
        var mission = Create();
        mission.AddShips(SpaceAssetType.ScoutCraft, int.MaxValue);
        Assert.Throws<OverflowException>(() => mission.AddShips(SpaceAssetType.ScoutCraft, 1));
        Assert.Equal(int.MaxValue, Assert.Single(mission.Ships).Quantity);
        Assert.Equal(1, mission.StateVersion);
    }

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft)]
    [InlineData(SpaceAssetType.CargoCraft)]
    public void VersionOverflowRejectsBothMergeAndNewRows(SpaceAssetType type)
    {
        using var db = Db();
        var mission = Create();
        mission.AddShips(SpaceAssetType.ScoutCraft, 6);
        db.Entry(mission).Property(x => x.StateVersion).CurrentValue = int.MaxValue;
        var before = Composition(mission);
        Assert.Throws<OverflowException>(() => mission.AddShips(type, 1));
        Assert.Equal(before, Composition(mission));
        Assert.Equal(int.MaxValue, mission.StateVersion);
    }

    [Theory]
    [InlineData(FleetMissionStatus.Outbound)]
    [InlineData(FleetMissionStatus.Processing)]
    [InlineData(FleetMissionStatus.Returning)]
    [InlineData(FleetMissionStatus.Completed)]
    [InlineData(FleetMissionStatus.Recalled)]
    [InlineData(FleetMissionStatus.Cancelled)]
    public void CompositionIsFrozenOutsidePreparing(FleetMissionStatus status)
    {
        var mission = Create();
        mission.AddShips(SpaceAssetType.ScoutCraft, 6);
        if (status == FleetMissionStatus.Cancelled) mission.Cancel(Now);
        else
        {
            mission.StartOutbound();
            if (status == FleetMissionStatus.Recalled)
            {
                mission.Recall(Now.AddMinutes(15), Now.AddMinutes(30));
                mission.Complete(Now.AddMinutes(30));
            }
            else if (status != FleetMissionStatus.Outbound)
            {
                mission.BeginProcessing(Now.AddHours(1));
                if (status == FleetMissionStatus.Returning) mission.BeginReturn(Now.AddHours(1), Now.AddHours(2));
                if (status == FleetMissionStatus.Completed) mission.Complete(Now.AddHours(1));
            }
        }
        var before = Composition(mission);
        var version = mission.StateVersion;
        Assert.Equal(status, mission.Status);
        Assert.Throws<InvalidOperationException>(() => mission.AddShips(SpaceAssetType.ScoutCraft, 1));
        Assert.Throws<InvalidOperationException>(() => mission.AddShips(SpaceAssetType.CargoCraft, 1));
        Assert.Equal(before, Composition(mission));
        Assert.Equal((status, version), (mission.Status, mission.StateVersion));
    }

    [Fact]
    public void ChildIdentityAndExternalEncapsulationAreEnforced()
    {
        Assert.Throws<ArgumentException>(() => FleetMissionShip.Create(Guid.Empty, SpaceAssetType.ScoutCraft, 1));
        var mission = Create();
        var view = Assert.IsAssignableFrom<ICollection<FleetMissionShip>>(mission.Ships);
        mission.AddShips(SpaceAssetType.ScoutCraft, 1);
        Assert.True(view.IsReadOnly);
        var ship = Assert.Single(view);
        Assert.Throws<NotSupportedException>(() => view.Add(ship));
        Assert.Throws<NotSupportedException>(() => view.Remove(ship));
        Assert.Throws<NotSupportedException>(() => view.Clear());
        Assert.All(typeof(FleetMissionShip).GetProperties(), property => Assert.True(property.SetMethod!.IsPrivate));
        Assert.Equal((1, 1), (Assert.Single(mission.Ships).Quantity, mission.StateVersion));
    }

    [Fact]
    public async Task EfMapsCompositeOwnershipAndRoundTripsMixedComposition()
    {
        await using var db = Db();
        var entity = db.Model.FindEntityType(typeof(FleetMissionShip))!;
        Assert.Equal(new[] { "MissionId", "AssetType" }, entity.FindPrimaryKey()!.Properties.Select(x => x.Name));
        Assert.All(entity.GetProperties(), property => Assert.False(property.IsNullable));
        var relationship = Assert.Single(entity.GetForeignKeys());
        Assert.Equal(typeof(FleetMission), relationship.PrincipalEntityType.ClrType);
        Assert.Equal("MissionId", Assert.Single(relationship.Properties).Name);
        Assert.True(relationship.IsRequired);
        Assert.False(relationship.IsUnique);
        Assert.Equal(DeleteBehavior.Cascade, relationship.DeleteBehavior);
        Assert.Equal(nameof(FleetMission.Ships), relationship.PrincipalToDependent!.Name);
        Assert.Equal("_ships", relationship.PrincipalToDependent.FieldInfo!.Name);
        var mission = Create();
        mission.AddShips(SpaceAssetType.ScoutCraft, 10);
        mission.AddShips(SpaceAssetType.EscortCraft, 5);
        mission.AddShips(SpaceAssetType.CargoCraft, 2);
        var expected = Composition(mission);
        db.FleetMissions.Add(mission);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var loaded = await db.FleetMissions.Include(x => x.Ships).SingleAsync();
        Assert.Equal(expected, Composition(loaded));
        Assert.All(loaded.Ships, ship => Assert.Equal(loaded.Id, ship.MissionId));
        Assert.Equal(3, loaded.StateVersion);
        loaded.AddShips(SpaceAssetType.ScoutCraft, 1);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        loaded = await db.FleetMissions.Include(x => x.Ships).SingleAsync();
        Assert.Equal(3, loaded.Ships.Count);
        Assert.Equal(11, loaded.Ships.Single(x => x.AssetType == SpaceAssetType.ScoutCraft).Quantity);
        Assert.Equal(4, loaded.StateVersion);
        db.FleetMissions.Remove(loaded);
        await db.SaveChangesAsync();
        Assert.Empty(await db.Set<FleetMissionShip>().ToListAsync());
        // InMemory validates mapping/materialization and tracked cascade, not relational constraints or races.
    }

    private static FleetMission Create() => FleetMission.CreatePreparing(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        FleetMissionType.Transport, Now, Now, Now.AddHours(1));
    private static (SpaceAssetType, int)[] Composition(FleetMission mission) =>
        mission.Ships.OrderBy(x => x.AssetType).Select(x => (x.AssetType, x.Quantity)).ToArray();
    private static VoidEmpiresDbContext Db() => new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}

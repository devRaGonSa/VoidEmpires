using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class FleetMissionCargoTests
{
    private static readonly DateTime Now = new(2026, 10, 8, 12, 0, 0, DateTimeKind.Utc);
    public static IEnumerable<object[]> Resources => Enum.GetValues<ResourceType>().Select(type => new object[] { type });
    public static IEnumerable<object[]> Phases => Enum.GetValues<FleetMissionStatus>().Select(status => new object[] { status });

    [Fact]
    public void AllResourcesAreNormalizedAndDuplicateLoadsMergeTheSameRow()
    {
        var mission = Create();
        foreach (var type in Enum.GetValues<ResourceType>())
        {
            var version = mission.StateVersion;
            mission.AddCargo(type, 600m);
            var cargo = mission.Cargo.Single(row => row.ResourceType == type);
            Assert.Equal(version + 1, mission.StateVersion);
            mission.AddCargo(type, 400m);
            Assert.Same(cargo, mission.Cargo.Single(row => row.ResourceType == type));
            Assert.Equal(version + 2, mission.StateVersion);
            Assert.Equal((mission.Id, 1000m, 0m, 0m, 1000m),
                (cargo.MissionId, cargo.LoadedAmount, cargo.DeliveredAmount, cargo.ReturnedAmount, cargo.RemainingAmount));
            AssertConservation(mission);
        }
        Assert.Equal((4, FleetMissionStatus.Preparing, 8), (mission.Cargo.Count, mission.Status, mission.StateVersion));
    }

    [Theory]
    [MemberData(nameof(Resources))]
    public void ZeroCreatesNoRowAndNegativeLoadsLeaveAllStateUntouched(ResourceType type)
    {
        var mission = Create();
        var empty = Snapshot(mission);
        mission.AddCargo(type, 0);
        Assert.Equal(empty, Snapshot(mission));
        Reject<ArgumentOutOfRangeException>(mission, () => mission.AddCargo(type, -0.0001m));
        mission.AddCargo(type, 123.4567m);
        Reject<ArgumentOutOfRangeException>(mission, () => mission.AddCargo(type, -1m));
        var loaded = Snapshot(mission);
        mission.AddCargo(type, 0);
        Assert.Equal(loaded, Snapshot(mission));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void UndefinedResourcesFailEvenForZeroCargo(int resource)
    {
        var mission = Create();
        mission.AddCargo(ResourceType.Metal, 1);
        Reject<ArgumentOutOfRangeException>(mission, () => mission.AddCargo((ResourceType)resource, 1));
        Reject<ArgumentOutOfRangeException>(mission, () => mission.AddCargo((ResourceType)resource, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetMissionCargo.Create(mission.Id, (ResourceType)resource, 1));
    }

    [Theory]
    [InlineData("0.0001")]
    [InlineData("1.0001")]
    [InlineData("1.2345")]
    [InlineData("123.4567")]
    [InlineData("1.234500")]
    [InlineData("99999999999999.9999")]
    public void ExactlyRepresentableAmountsAreNeverRounded(string value)
    {
        var amount = Amount(value);
        var mission = Create();
        mission.AddCargo(ResourceType.Crystal, amount);
        Assert.Equal(amount, Assert.Single(mission.Cargo).LoadedAmount);
        AssertConservation(mission);
    }

    [Theory]
    [InlineData("0.00001")]
    [InlineData("123.45678")]
    [InlineData("0.0000000000000000000000000001")]
    public void UnsupportedScaleIsRejectedBeforeAddingOrMerging(string value)
    {
        var mission = Create();
        var amount = Amount(value);
        Reject<ArgumentException>(mission, () => mission.AddCargo(ResourceType.Metal, amount));
        mission.AddCargo(ResourceType.Metal, 10);
        Reject<ArgumentException>(mission, () => mission.AddCargo(ResourceType.Metal, amount));
    }

    [Fact]
    public void StorageOverflowAndCheckedClrAdditionOverflowAreAtomic()
    {
        var mission = Create();
        Reject<OverflowException>(mission, () => mission.AddCargo(ResourceType.Metal, FleetMissionCargo.MaximumAmount + 0.0001m));
        Reject<OverflowException>(mission, () => mission.AddCargo(ResourceType.Gas, decimal.MaxValue));
        mission.AddCargo(ResourceType.Metal, FleetMissionCargo.MaximumAmount - 0.0001m);
        mission.AddCargo(ResourceType.Metal, 0.0001m);
        Assert.Equal(FleetMissionCargo.MaximumAmount, Assert.Single(mission.Cargo).LoadedAmount);
        Reject<OverflowException>(mission, () => mission.AddCargo(ResourceType.Metal, 0.0001m));

        // Valid decimal(18,4) operands cannot overflow CLR decimal. Inject corruption only to exercise checked addition.
        using var db = Db();
        db.Entry(Assert.Single(mission.Cargo)).Property(row => row.LoadedAmount).CurrentValue = decimal.MaxValue;
        Reject<OverflowException>(mission, () => mission.AddCargo(ResourceType.Metal, 1));
    }

    [Theory]
    [MemberData(nameof(Phases))]
    public void PositiveLoadingIsPreparingOnlyButZeroIsAlwaysANoOp(FleetMissionStatus status)
    {
        var mission = AtPhase(status);
        if (status == FleetMissionStatus.Preparing)
        {
            mission.AddCargo(ResourceType.Metal, 1);
            Assert.Equal(101m, Assert.Single(mission.Cargo).LoadedAmount);
        }
        else
        {
            Reject<InvalidOperationException>(mission, () => mission.AddCargo(ResourceType.Metal, 1));
            Reject<InvalidOperationException>(mission, () => mission.AddCargo(ResourceType.Gas, 1));
        }
        var before = Snapshot(mission);
        mission.AddCargo(ResourceType.Metal, 0);
        mission.AddCargo(ResourceType.Gas, 0);
        Assert.Equal(before, Snapshot(mission));
    }

    [Theory]
    [MemberData(nameof(Phases))]
    public void DeliveryAndReturnAreRestrictedToTheirAccountingPhase(FleetMissionStatus status)
    {
        var mission = AtPhase(status);
        if (status == FleetMissionStatus.Processing) Assert.True(mission.RecordCargoDelivered(ResourceType.Metal, 1));
        else Reject<InvalidOperationException>(mission, () => mission.RecordCargoDelivered(ResourceType.Metal, 1));
        if (status == FleetMissionStatus.Returning) Assert.True(mission.RecordCargoReturned(ResourceType.Metal, 1));
        else Reject<InvalidOperationException>(mission, () => mission.RecordCargoReturned(ResourceType.Metal, 1));
        Assert.Equal(status, mission.Status);
        AssertConservation(mission);
    }

    [Theory]
    [MemberData(nameof(Resources))]
    public void PartialDeliveryAndReturnConserveEveryResourceAndVersionEachEffectOnce(ResourceType type)
    {
        var mission = Create();
        mission.AddCargo(type, 1000);
        mission.StartOutbound();
        mission.BeginProcessing(Now.AddHours(1));
        Assert.True(mission.RecordCargoDelivered(type, 700));
        Assert.Equal((1000m, 700m, 0m, 300m), Ledger(Assert.Single(mission.Cargo)));
        Assert.Equal(4, mission.StateVersion);
        AssertConservation(mission);
        var delivered = Snapshot(mission);
        Assert.False(mission.RecordCargoDelivered(type, 700));
        Assert.Equal(delivered, Snapshot(mission));
        Reject<InvalidOperationException>(mission, () => mission.RecordCargoDelivered(type, 300.0001m, 700));
        mission.BeginReturn(Now.AddHours(1), Now.AddHours(2));
        Assert.True(mission.RecordCargoReturned(type, 300));
        Assert.Equal((1000m, 700m, 300m, 0m), Ledger(Assert.Single(mission.Cargo)));
        Assert.Equal(6, mission.StateVersion);
        AssertConservation(mission);
        var returned = Snapshot(mission);
        Assert.False(mission.RecordCargoReturned(type, 300));
        Assert.Equal(returned, Snapshot(mission));
        Reject<InvalidOperationException>(mission, () => mission.RecordCargoReturned(type, 0.0001m, 300));
        Reject<InvalidOperationException>(mission, () => mission.RecordCargoReturned(type, 300, 300));
        mission.Complete(Now.AddHours(2));
        Reject<InvalidOperationException>(mission, () => mission.RecordCargoReturned(type, 300));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EqualInstallmentsRequireExplicitExpectedCountersAndStaleRetriesFail(bool returning)
    {
        var mission = AtPhase(returning ? FleetMissionStatus.Returning : FleetMissionStatus.Processing);
        var initialVersion = mission.StateVersion;
        Assert.True(Settle(mission, returning, 20));
        AssertConservation(mission);
        var once = Snapshot(mission);
        Assert.False(Settle(mission, returning, 20));
        Assert.Equal(once, Snapshot(mission));
        Reject<InvalidOperationException>(mission, () => Settle(mission, returning, 30));
        Reject<InvalidOperationException>(mission, () => Settle(mission, returning, 1, 21));
        Assert.True(Settle(mission, returning, 20, 20));
        AssertConservation(mission);
        Reject<InvalidOperationException>(mission, () => Settle(mission, returning, 20));
        var twice = Snapshot(mission);
        Assert.False(Settle(mission, returning, 20, 20));
        Assert.Equal(twice, Snapshot(mission));
        Assert.Equal(initialVersion + 2, mission.StateVersion);
        Assert.Equal(60m, Assert.Single(mission.Cargo).RemainingAmount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InvalidSettlementInputsAndExpectationsNeverChangeAnyState(bool returning)
    {
        var mission = AtPhase(returning ? FleetMissionStatus.Returning : FleetMissionStatus.Processing);
        Assert.True(Settle(mission, returning, 20));
        Reject<ArgumentOutOfRangeException>(mission, () => Settle(mission, returning, 0, 20));
        Reject<ArgumentOutOfRangeException>(mission, () => Settle(mission, returning, -1, 20));
        Reject<ArgumentOutOfRangeException>(mission, () => Settle(mission, returning, 1, -1));
        Reject<ArgumentException>(mission, () => Settle(mission, returning, 0.00001m, 20));
        Reject<ArgumentException>(mission, () => Settle(mission, returning, 1, 20.00001m));
        Reject<OverflowException>(mission, () => Settle(mission, returning, decimal.MaxValue, 20));
        Reject<OverflowException>(mission, () => Settle(mission, returning, 1, decimal.MaxValue));
        Reject<OverflowException>(mission, () => Settle(mission, returning, 0.0001m, FleetMissionCargo.MaximumAmount));
        Reject<InvalidOperationException>(mission, () => Settle(mission, returning, 80.0001m, 20));
        Reject<InvalidOperationException>(mission, () => Settle(mission, returning, 1, resourceType: ResourceType.Crystal));
        Reject<ArgumentOutOfRangeException>(mission, () => Settle(mission, returning, 1, resourceType: (ResourceType)99));
    }

    [Fact]
    public void RecallKeepsTheSameMissionAndAllCargoUntilActualReturnAccounting()
    {
        var mission = Create();
        mission.AddCargo(ResourceType.Gas, 123.4567m);
        var identity = mission.Id;
        mission.StartOutbound();
        mission.Recall(Now.AddMinutes(15), Now.AddMinutes(30));
        Assert.Equal((123.4567m, 0m, 0m, 123.4567m), Ledger(Assert.Single(mission.Cargo)));
        AssertConservation(mission);
        Assert.True(mission.RecordCargoReturned(ResourceType.Gas, 123.4567m));
        AssertConservation(mission);
        var once = Snapshot(mission);
        Assert.False(mission.RecordCargoReturned(ResourceType.Gas, 123.4567m));
        Assert.Equal(once, Snapshot(mission));
        mission.Complete(Now.AddMinutes(30));
        Assert.Equal((identity, FleetMissionStatus.Recalled), (mission.Id, mission.Status));
        Assert.Equal((123.4567m, 0m, 123.4567m, 0m), Ledger(Assert.Single(mission.Cargo)));
        Reject<InvalidOperationException>(mission, () => mission.RecordCargoReturned(ResourceType.Gas, 123.4567m));
    }

    [Fact]
    public void AccountingIsIsolatedByResourceAndMissionAndAddsNoEligibilityPolicy()
    {
        foreach (var missionType in Enum.GetValues<FleetMissionType>())
        {
            var mission = Create(missionType);
            var other = Create();
            foreach (var type in Enum.GetValues<ResourceType>())
            {
                mission.AddCargo(type, 100);
                other.AddCargo(type, 100);
            }
            var otherBefore = Snapshot(other);
            mission.StartOutbound();
            mission.BeginProcessing(Now.AddHours(1));
            mission.RecordCargoDelivered(ResourceType.Metal, 70);
            Assert.All(mission.Cargo.Where(row => row.ResourceType != ResourceType.Metal),
                row => Assert.Equal((100m, 0m, 0m, 100m), Ledger(row)));
            mission.BeginReturn(Now.AddHours(1), Now.AddHours(2));
            mission.RecordCargoReturned(ResourceType.Metal, 30);
            AssertConservation(mission);
            Assert.Equal(otherBefore, Snapshot(other));
        }
    }

    [Theory]
    [InlineData("new")]
    [InlineData("merge")]
    [InlineData("delivery")]
    [InlineData("return")]
    public void VersionOverflowPrecedesEveryCargoMutation(string operation)
    {
        var mission = AtPhase(operation switch
        {
            "delivery" => FleetMissionStatus.Processing,
            "return" => FleetMissionStatus.Returning,
            _ => FleetMissionStatus.Preparing
        });
        using var db = Db();
        db.Entry(mission).Property(row => row.StateVersion).CurrentValue = int.MaxValue;
        Reject<OverflowException>(mission, () =>
        {
            if (operation == "new") mission.AddCargo(ResourceType.Gas, 1);
            else if (operation == "merge") mission.AddCargo(ResourceType.Metal, 1);
            else Settle(mission, operation == "return", 1);
        });
        var before = Snapshot(mission);
        mission.AddCargo(ResourceType.Metal, 0);
        mission.AddCargo(ResourceType.Gas, 0);
        Assert.Equal(before, Snapshot(mission));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ExactReplayDoesNotTryToIncrementAnExhaustedVersion(bool returning)
    {
        var mission = AtPhase(returning ? FleetMissionStatus.Returning : FleetMissionStatus.Processing);
        Assert.True(Settle(mission, returning, 100));
        using var db = Db();
        db.Entry(mission).Property(row => row.StateVersion).CurrentValue = int.MaxValue;
        var before = Snapshot(mission);
        Assert.False(Settle(mission, returning, 100));
        Assert.Equal(before, Snapshot(mission));
        AssertConservation(mission);
    }

    [Theory]
    [MemberData(nameof(Resources))]
    public void SuppliedAvailabilityChecksExactAndSmallestUnitOverWithoutSpending(ResourceType type)
    {
        var mission = Create();
        var stockpile = PlanetResourceStockpile.Create(mission.OriginPlanetId, lastAccruedAtUtc: Now);
        stockpile.Increase(type, 100);
        var stockBefore = JsonSerializer.Serialize(stockpile);
        mission.AddCargo(type, 100);
        var before = Snapshot(mission);
        mission.ValidateCargoAvailability(type, 100);
        Reject<InvalidOperationException>(mission, () => mission.ValidateCargoAvailability(type, 99.9999m));
        Assert.Equal(before, Snapshot(mission));
        mission.AddCargo(type, 0.0001m);
        Reject<InvalidOperationException>(mission, () => mission.ValidateCargoAvailability(type, 100));
        Assert.Equal(stockBefore, JsonSerializer.Serialize(stockpile));
        Assert.True(stockpile.HasAtLeast(type, 100));
    }

    [Fact]
    public void GasAndFuelShareOneAvailabilityCheckWithoutTurningFuelIntoCargo()
    {
        var mission = Create();
        mission.ValidateCargoAvailability(ResourceType.Gas, 100, 100);
        Assert.Empty(mission.Cargo);
        Assert.Equal(0, mission.StateVersion);
        mission.AddCargo(ResourceType.Gas, 70);
        var before = Snapshot(mission);
        mission.ValidateCargoAvailability(ResourceType.Gas, 100, 30);
        Reject<InvalidOperationException>(mission, () => mission.ValidateCargoAvailability(ResourceType.Gas, 100, 30.0001m));
        Assert.Equal(before, Snapshot(mission));
        Assert.Equal((70m, 0m, 0m, 70m), Ledger(Assert.Single(mission.Cargo)));
    }

    [Fact]
    public void AvailabilityRejectsInvalidInputsAndHandlesAbsentResourcesAndMaximumCombinedCost()
    {
        var mission = Create();
        foreach (var type in Enum.GetValues<ResourceType>())
        {
            mission.ValidateCargoAvailability(type, 0);
            Reject<ArgumentOutOfRangeException>(mission, () => mission.ValidateCargoAvailability(type, -1));
            Reject<ArgumentException>(mission, () => mission.ValidateCargoAvailability(type, 0.00001m));
            Reject<OverflowException>(mission, () => mission.ValidateCargoAvailability(type, decimal.MaxValue));
        }
        Reject<ArgumentOutOfRangeException>(mission, () => mission.ValidateCargoAvailability((ResourceType)99, 0));
        Reject<ArgumentOutOfRangeException>(mission, () => mission.ValidateCargoAvailability(ResourceType.Gas, 1, -1));
        Reject<ArgumentException>(mission, () => mission.ValidateCargoAvailability(ResourceType.Gas, 1, 0.00001m));
        Reject<OverflowException>(mission, () => mission.ValidateCargoAvailability(ResourceType.Gas, 1, decimal.MaxValue));
        Reject<ArgumentException>(mission, () => mission.ValidateCargoAvailability(ResourceType.Metal, 1, 1));
        mission.AddCargo(ResourceType.Gas, FleetMissionCargo.MaximumAmount);
        mission.ValidateCargoAvailability(ResourceType.Gas, FleetMissionCargo.MaximumAmount);
        Reject<InvalidOperationException>(mission, () => mission.ValidateCargoAvailability(ResourceType.Gas,
            FleetMissionCargo.MaximumAmount, FleetMissionCargo.MaximumAmount));
    }

    [Fact]
    public void CapacityUsesOnlySuppliedNonnegativeUnitsWithoutAResourceConversion()
    {
        FleetMission.ValidateCargoCapacity(0, 0);
        FleetMission.ValidateCargoCapacity(123.45678m, 123.45678m);
        FleetMission.ValidateCargoCapacity(0, decimal.MaxValue);
        FleetMission.ValidateCargoCapacity(decimal.MaxValue, decimal.MaxValue);
        Assert.Throws<InvalidOperationException>(() => FleetMission.ValidateCargoCapacity(100.0001m, 100));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetMission.ValidateCargoCapacity(-0.0001m, 100));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetMission.ValidateCargoCapacity(0, -0.0001m));
    }

    [Fact]
    public void ChildIdentityPositiveRowsAndExternalEncapsulationAreEnforced()
    {
        Assert.Throws<ArgumentException>(() => FleetMissionCargo.Create(Guid.Empty, ResourceType.Metal, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetMissionCargo.Create(Guid.NewGuid(), ResourceType.Metal, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetMissionCargo.Create(Guid.NewGuid(), ResourceType.Metal, -1));
        Assert.Throws<ArgumentException>(() => FleetMissionCargo.Create(Guid.NewGuid(), ResourceType.Metal, 0.00001m));
        Assert.Throws<OverflowException>(() => FleetMissionCargo.Create(Guid.NewGuid(), ResourceType.Metal, decimal.MaxValue));
        var mission = Create();
        var view = Assert.IsAssignableFrom<ICollection<FleetMissionCargo>>(mission.Cargo);
        Assert.False(mission.Cargo is List<FleetMissionCargo>);
        mission.AddCargo(ResourceType.Metal, 1);
        Assert.Same(view, mission.Cargo);
        Assert.True(view.IsReadOnly);
        var row = Assert.Single(view);
        var before = Snapshot(mission);
        Assert.Throws<NotSupportedException>(() => view.Add(row));
        Assert.Throws<NotSupportedException>(() => view.Remove(row));
        Assert.Throws<NotSupportedException>(() => view.Clear());
        Assert.All(typeof(FleetMissionCargo).GetProperties().Where(property => property.Name != nameof(FleetMissionCargo.RemainingAmount)),
            property => Assert.True(property.SetMethod!.IsPrivate));
        Assert.Null(typeof(FleetMissionCargo).GetProperty(nameof(FleetMissionCargo.RemainingAmount))!.SetMethod);
        Assert.Equal(before, Snapshot(mission));
    }

    [Fact]
    public void EfMapsNormalizedOwnershipPrecisionAndExistingParentConcurrency()
    {
        using var db = Db();
        var entity = db.Model.FindEntityType(typeof(FleetMissionCargo))!;
        Assert.Equal(new[] { "MissionId", "ResourceType" }, entity.FindPrimaryKey()!.Properties.Select(property => property.Name));
        Assert.Equal(5, entity.GetProperties().Count());
        Assert.All(entity.GetProperties(), property => Assert.False(property.IsNullable));
        foreach (var name in new[] { "LoadedAmount", "DeliveredAmount", "ReturnedAmount" })
        {
            Assert.Equal(18, entity.FindProperty(name)!.GetPrecision());
            Assert.Equal(4, entity.FindProperty(name)!.GetScale());
            Assert.Equal(typeof(decimal), entity.FindProperty(name)!.ClrType);
        }
        Assert.Null(entity.FindProperty(nameof(FleetMissionCargo.RemainingAmount)));
        var relationship = Assert.Single(entity.GetForeignKeys());
        Assert.Equal(typeof(FleetMission), relationship.PrincipalEntityType.ClrType);
        Assert.Equal("MissionId", Assert.Single(relationship.Properties).Name);
        Assert.True(relationship.IsRequired);
        Assert.False(relationship.IsUnique);
        Assert.Equal(DeleteBehavior.Cascade, relationship.DeleteBehavior);
        Assert.Equal(nameof(FleetMission.Cargo), relationship.PrincipalToDependent!.Name);
        Assert.Equal("_cargo", relationship.PrincipalToDependent.FieldInfo!.Name);
        Assert.Equal(PropertyAccessMode.Field, relationship.PrincipalToDependent.GetPropertyAccessMode());
        Assert.True(relationship.PrincipalEntityType.FindProperty(nameof(FleetMission.StateVersion))!.IsConcurrencyToken);
    }

    [Fact]
    public async Task EfRoundTripsAllResourcesAndPartialSettlementWithPersistedReplayAndTrackedCascade()
    {
        await using var db = Db();
        var mission = Create();
        mission.AddCargo(ResourceType.Credits, 0.0001m);
        mission.AddCargo(ResourceType.Metal, 1000m);
        mission.AddCargo(ResourceType.Crystal, 123.4567m);
        mission.AddCargo(ResourceType.Gas, 1.2345m);
        var expected = Snapshot(mission);
        db.FleetMissions.Add(mission);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var loaded = await db.FleetMissions.Include(row => row.Cargo).SingleAsync();
        Assert.NotSame(mission, loaded);
        Assert.Equal(expected, Snapshot(loaded));
        Assert.Equal(4, loaded.Cargo.Count);
        Assert.All(loaded.Cargo, row => Assert.Equal(loaded.Id, row.MissionId));
        loaded.StartOutbound();
        loaded.BeginProcessing(Now.AddHours(1));
        loaded.RecordCargoDelivered(ResourceType.Metal, 700);
        loaded.RecordCargoDelivered(ResourceType.Crystal, 23.4567m);
        AssertConservation(loaded);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        loaded = await db.FleetMissions.Include(row => row.Cargo).SingleAsync();
        var delivered = Snapshot(loaded);
        Assert.False(loaded.RecordCargoDelivered(ResourceType.Crystal, 23.4567m));
        Assert.Equal(delivered, Snapshot(loaded));
        loaded.BeginReturn(Now.AddHours(1), Now.AddHours(2));
        loaded.RecordCargoReturned(ResourceType.Metal, 100.0001m);
        loaded.RecordCargoReturned(ResourceType.Gas, 1.2345m);
        expected = Snapshot(loaded);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        loaded = await db.FleetMissions.Include(row => row.Cargo).SingleAsync();
        Assert.Equal(expected, Snapshot(loaded));
        Assert.False(loaded.RecordCargoReturned(ResourceType.Metal, 100.0001m));
        Assert.Equal(expected, Snapshot(loaded));
        Assert.True(loaded.RecordCargoReturned(ResourceType.Metal, 199.9999m, 100.0001m));
        Assert.True(loaded.RecordCargoReturned(ResourceType.Credits, 0.0001m));
        Assert.True(loaded.RecordCargoReturned(ResourceType.Crystal, 100m));
        Assert.All(loaded.Cargo, row => Assert.Equal(0, row.RemainingAmount));
        AssertConservation(loaded);
        loaded.Complete(Now.AddHours(2));
        expected = Snapshot(loaded);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        loaded = await db.FleetMissions.Include(row => row.Cargo).SingleAsync();
        Assert.Equal(expected, Snapshot(loaded));
        db.FleetMissions.Remove(loaded);
        await db.SaveChangesAsync();
        Assert.Empty(await db.Set<FleetMissionCargo>().ToListAsync());
        // InMemory proves materialization/metadata/tracked cascade, not relational constraints, races or rollback.
    }

    private static FleetMission Create(FleetMissionType type = FleetMissionType.Transport) =>
        FleetMission.CreatePreparing(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), type, Now, Now, Now.AddHours(1));

    private static FleetMission AtPhase(FleetMissionStatus status)
    {
        var mission = Create();
        mission.AddCargo(ResourceType.Metal, 100);
        if (status == FleetMissionStatus.Preparing) return mission;
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
        Assert.Equal(status, mission.Status);
        return mission;
    }

    private static bool Settle(FleetMission mission, bool returning, decimal amount, decimal expected = 0,
        ResourceType resourceType = ResourceType.Metal) => returning
        ? mission.RecordCargoReturned(resourceType, amount, expected)
        : mission.RecordCargoDelivered(resourceType, amount, expected);
    private static decimal Amount(string value) => decimal.Parse(value, CultureInfo.InvariantCulture);
    private static (decimal, decimal, decimal, decimal) Ledger(FleetMissionCargo row) =>
        (row.LoadedAmount, row.DeliveredAmount, row.ReturnedAmount, row.RemainingAmount);
    private static string Snapshot(FleetMission mission) => JsonSerializer.Serialize(mission);
    private static void AssertConservation(FleetMission mission) => Assert.All(mission.Cargo, row =>
    {
        Assert.True(row.LoadedAmount > 0);
        Assert.True(row.DeliveredAmount >= 0 && row.ReturnedAmount >= 0 && row.RemainingAmount >= 0);
        Assert.Equal(row.LoadedAmount, row.DeliveredAmount + row.ReturnedAmount + row.RemainingAmount);
    });
    private static void Reject<T>(FleetMission mission, Action operation) where T : Exception
    {
        var before = Snapshot(mission);
        Assert.Throws<T>(operation);
        Assert.Equal(before, Snapshot(mission));
        AssertConservation(mission);
    }
    private static VoidEmpiresDbContext Db() => new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}

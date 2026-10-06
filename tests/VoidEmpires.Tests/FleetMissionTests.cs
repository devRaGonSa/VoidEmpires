using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class FleetMissionTests
{
    private static readonly Guid Civilization = Guid.NewGuid(), Origin = Guid.NewGuid(), Destination = Guid.NewGuid();
    private static readonly DateTime Created = new(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime Departure = Created.AddMinutes(1), Arrival = Departure.AddHours(1);
    private static readonly DateTime ReturnArrival = Arrival.AddHours(1);
    private static readonly DateTime RecallAt = Departure.AddMinutes(15), RecallArrival = Departure.AddMinutes(30);

    [Theory]
    [InlineData(FleetMissionType.Deploy)]
    [InlineData(FleetMissionType.Transport)]
    [InlineData(FleetMissionType.Expedition)]
    [InlineData(FleetMissionType.Colonization)]
    public void CreationRetainsIdentityTypeAndUtcSchedule(FleetMissionType type)
    {
        var mission = Create(type);
        Assert.NotEqual(Guid.Empty, mission.Id);
        Assert.NotEqual(mission.Id, Create(type).Id);
        Assert.Equal((Civilization, Origin, Destination, type),
            (mission.CivilizationId, mission.OriginPlanetId, mission.DestinationPlanetId, mission.MissionType));
        Assert.Equal((Created, Departure, Arrival), (mission.CreatedAtUtc, mission.OutboundDepartureUtc, mission.OutboundArrivalUtc));
        Assert.Equal((FleetMissionStatus.Preparing, 0), (mission.Status, mission.StateVersion));
        Assert.Null(mission.ReturnDepartureUtc);
        Assert.Null(mission.ReturnArrivalUtc);
        Assert.Null(mission.CompletedAtUtc);
        Assert.Null(mission.RecalledAtUtc);
        Assert.All(typeof(FleetMission).GetProperties(), property => Assert.True(property.SetMethod!.IsPrivate));
    }

    [Theory]
    [InlineData("civilization")]
    [InlineData("origin")]
    [InlineData("destination")]
    [InlineData("samePlanet")]
    public void CreationRejectsInvalidIdentifiers(string invalid)
    {
        Assert.Throws<ArgumentException>(() => FleetMission.CreatePreparing(
            invalid == "civilization" ? Guid.Empty : Civilization,
            invalid == "origin" ? Guid.Empty : Origin,
            invalid == "destination" ? Guid.Empty : invalid == "samePlanet" ? Origin : Destination,
            FleetMissionType.Deploy, Created, Departure, Arrival));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public void CreationRejectsUnknownMissionTypes(int type) => Assert.Throws<ArgumentException>(() => Create((FleetMissionType)type));

    [Theory]
    [InlineData(1, 60)]
    [InlineData(-1, 0)]
    [InlineData(-1, -1)]
    public void CreationRejectsInvalidOrdering(int createdMinutes, int arrivalMinutes)
    {
        Assert.Throws<ArgumentException>(() => FleetMission.CreatePreparing(Civilization, Origin, Destination,
            FleetMissionType.Deploy, Departure.AddMinutes(createdMinutes), Departure, Departure.AddMinutes(arrivalMinutes)));
    }

    [Fact]
    public void CreationMayCoincideWithDeparture()
    {
        var mission = FleetMission.CreatePreparing(Civilization, Origin, Destination, FleetMissionType.Deploy, Departure, Departure, Arrival);
        Assert.Equal(mission.CreatedAtUtc, mission.OutboundDepartureUtc);
    }

    public static IEnumerable<object[]> NonUtcCases() =>
        from field in new[] { "created", "outboundDeparture", "outboundArrival", "processing", "returnDeparture", "returnArrival", "recall", "recallArrival", "completion", "cancellation" }
        from kind in new[] { DateTimeKind.Unspecified, DateTimeKind.Local }
        select new object[] { field, kind };

    [Theory]
    [MemberData(nameof(NonUtcCases))]
    public void EveryAcceptedTimestampRejectsNonUtc(string field, DateTimeKind kind)
    {
        DateTime Bad(DateTime value) => DateTime.SpecifyKind(value, kind);
        var mission = InState(field switch
        {
            "processing" or "recall" or "recallArrival" => FleetMissionStatus.Outbound,
            "returnDeparture" or "returnArrival" or "completion" => FleetMissionStatus.Processing,
            _ => FleetMissionStatus.Preparing
        });
        Action operation = field switch
        {
            "created" => () => FleetMission.CreatePreparing(Civilization, Origin, Destination, FleetMissionType.Deploy, Bad(Created), Departure, Arrival),
            "outboundDeparture" => () => FleetMission.CreatePreparing(Civilization, Origin, Destination, FleetMissionType.Deploy, Created, Bad(Departure), Arrival),
            "outboundArrival" => () => FleetMission.CreatePreparing(Civilization, Origin, Destination, FleetMissionType.Deploy, Created, Departure, Bad(Arrival)),
            "processing" => () => mission.BeginProcessing(Bad(Arrival)),
            "returnDeparture" => () => mission.BeginReturn(Bad(Arrival), ReturnArrival),
            "returnArrival" => () => mission.BeginReturn(Arrival, Bad(ReturnArrival)),
            "recall" => () => mission.Recall(Bad(RecallAt), RecallArrival),
            "recallArrival" => () => mission.Recall(RecallAt, Bad(RecallArrival)),
            "completion" => () => mission.Complete(Bad(Arrival)),
            "cancellation" => () => mission.Cancel(Bad(Created)),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };
        RejectWithoutMutation<ArgumentException>(mission, operation);
    }

    [Fact]
    public void OneWayCompletionRequiresArrivalProcessingAndSettlement()
    {
        var mission = Create();
        mission.StartOutbound();
        Assert.Equal((FleetMissionStatus.Outbound, 1), (mission.Status, mission.StateVersion));
        RejectWithoutMutation<InvalidOperationException>(mission, () => mission.BeginProcessing(Arrival.AddTicks(-1)));
        RejectWithoutMutation<InvalidOperationException>(mission, () => mission.Complete(Arrival));
        mission.BeginProcessing(Arrival);
        Assert.Equal((FleetMissionStatus.Processing, 2), (mission.Status, mission.StateVersion));
        RejectWithoutMutation<InvalidOperationException>(mission, () => mission.Complete(Arrival.AddTicks(-1)));
        mission.Complete(Arrival);
        Assert.Equal((FleetMissionStatus.Completed, 3), (mission.Status, mission.StateVersion));
        Assert.Equal(Arrival, mission.CompletedAtUtc);
        Assert.Null(mission.ReturnDepartureUtc);
        Assert.Null(mission.ReturnArrivalUtc);
    }

    [Theory]
    [InlineData(FleetMissionType.Deploy)]
    [InlineData(FleetMissionType.Transport)]
    [InlineData(FleetMissionType.Expedition)]
    [InlineData(FleetMissionType.Colonization)]
    public void OrdinaryReturnBelongsToTheSameMissionAndSettlesOnce(FleetMissionType type)
    {
        var mission = Create(type);
        var id = mission.Id;
        mission.StartOutbound();
        mission.BeginProcessing(Arrival);
        mission.BeginReturn(Arrival, ReturnArrival);
        Assert.Equal((FleetMissionStatus.Returning, 3), (mission.Status, mission.StateVersion));
        Assert.Equal((Arrival, ReturnArrival), (mission.ReturnDepartureUtc, mission.ReturnArrivalUtc));
        Assert.Null(mission.RecalledAtUtc);
        Assert.Null(mission.CompletedAtUtc);
        var returning = Snapshot(mission);
        mission.BeginReturn(Arrival, ReturnArrival);
        Assert.Equal(returning, Snapshot(mission));
        RejectWithoutMutation<InvalidOperationException>(mission, () => mission.Complete(ReturnArrival.AddTicks(-1)));
        mission.Complete(ReturnArrival);
        Assert.Equal((FleetMissionStatus.Completed, 4), (mission.Status, mission.StateVersion));
        Assert.Equal(ReturnArrival, mission.CompletedAtUtc);
        Assert.Equal((id, Origin, Destination), (mission.Id, mission.OriginPlanetId, mission.DestinationPlanetId));
    }

    [Theory]
    [InlineData(FleetMissionType.Deploy)]
    [InlineData(FleetMissionType.Transport)]
    [InlineData(FleetMissionType.Expedition)]
    [InlineData(FleetMissionType.Colonization)]
    public void RecallRemainsReturningUntilPhysicalReturnSettles(FleetMissionType type)
    {
        var mission = Create(type);
        var id = mission.Id;
        mission.StartOutbound();
        mission.Recall(RecallAt, RecallArrival);
        Assert.Equal((FleetMissionStatus.Returning, 2), (mission.Status, mission.StateVersion));
        Assert.Equal((RecallAt, RecallArrival), (mission.ReturnDepartureUtc, mission.ReturnArrivalUtc));
        Assert.Equal(RecallAt, mission.RecalledAtUtc);
        Assert.Null(mission.CompletedAtUtc);
        var returning = Snapshot(mission);
        mission.Recall(RecallAt, RecallArrival);
        Assert.Equal(returning, Snapshot(mission));
        RejectWithoutMutation<InvalidOperationException>(mission, () => mission.Complete(RecallArrival.AddTicks(-1)));
        mission.Complete(RecallArrival);
        Assert.Equal((FleetMissionStatus.Recalled, 3), (mission.Status, mission.StateVersion));
        Assert.Equal(RecallArrival, mission.CompletedAtUtc);
        Assert.Equal(id, mission.Id);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 0)]
    [InlineData(0, -1)]
    public void NormalReturnRejectsImpossibleTimes(int departureTicks, int arrivalTicks)
    {
        var mission = InState(FleetMissionStatus.Processing);
        RejectWithoutMutation<InvalidOperationException>(mission,
            () => mission.BeginReturn(Arrival.AddTicks(departureTicks), Arrival.AddTicks(arrivalTicks)));
    }

    [Theory]
    [InlineData(-1, 30)]
    [InlineData(0, 0)]
    [InlineData(15, 15)]
    [InlineData(15, 14)]
    [InlineData(60, 120)]
    [InlineData(61, 120)]
    public void RecallRejectsImpossibleTimesIncludingTheArrivalBoundary(int recallMinutes, int arrivalMinutes)
    {
        var mission = InState(FleetMissionStatus.Outbound);
        RejectWithoutMutation<InvalidOperationException>(mission,
            () => mission.Recall(Departure.AddMinutes(recallMinutes), Departure.AddMinutes(arrivalMinutes)));
    }

    [Theory]
    [InlineData(false, 1, 0)]
    [InlineData(false, 0, 1)]
    [InlineData(true, 1, 0)]
    [InlineData(true, 0, 1)]
    public void AReturnScheduleCannotBeReplaced(bool recall, int departureTicks, int arrivalTicks)
    {
        var mission = InState(recall ? FleetMissionStatus.Outbound : FleetMissionStatus.Processing);
        Action<DateTime, DateTime> schedule = recall ? mission.Recall : mission.BeginReturn;
        var departure = recall ? RecallAt : Arrival;
        var arrival = recall ? RecallArrival : ReturnArrival;
        schedule(departure, arrival);
        RejectWithoutMutation<InvalidOperationException>(mission, () => schedule(departure.AddTicks(departureTicks), arrival.AddTicks(arrivalTicks)));
        RejectWithoutMutation<InvalidOperationException>(mission,
            () => { if (recall) mission.BeginReturn(Arrival, ReturnArrival); else mission.Recall(RecallAt, RecallArrival); });
    }

    [Fact]
    public void CancellationRequiresPreparingAndCannotPredateCreation()
    {
        var mission = Create();
        RejectWithoutMutation<InvalidOperationException>(mission, () => mission.Cancel(Created.AddTicks(-1)));
        mission.Cancel(Created);
        Assert.Equal((FleetMissionStatus.Cancelled, 1), (mission.Status, mission.StateVersion));
        Assert.Equal(Created, mission.CompletedAtUtc);
        Assert.Null(mission.ReturnDepartureUtc);
        Assert.Null(mission.RecalledAtUtc);
    }

    [Theory]
    [InlineData(FleetMissionStatus.Completed)]
    [InlineData(FleetMissionStatus.Recalled)]
    [InlineData(FleetMissionStatus.Cancelled)]
    public void TerminalSettlementOnlyAcceptsExactIdempotentRepeats(FleetMissionStatus status)
    {
        var mission = InState(status);
        Action<DateTime> settle = status == FleetMissionStatus.Cancelled ? mission.Cancel : mission.Complete;
        var before = Snapshot(mission);
        var settledAt = mission.CompletedAtUtc!.Value;
        settle(settledAt);
        Assert.Equal(before, Snapshot(mission));
        RejectWithoutMutation<InvalidOperationException>(mission, () => settle(settledAt.AddTicks(1)));
        RejectWithoutMutation<InvalidOperationException>(mission, () => settle(settledAt.AddTicks(-1)));
        RejectWithoutMutation<ArgumentException>(mission, () => settle(DateTime.SpecifyKind(settledAt, DateTimeKind.Unspecified)));
        RejectWithoutMutation<ArgumentException>(mission, () => settle(DateTime.SpecifyKind(settledAt, DateTimeKind.Local)));
    }

    public static IEnumerable<object[]> IllegalTransitions()
    {
        foreach (var status in Enum.GetValues<FleetMissionStatus>())
        {
            string[] allowed = status switch
            {
                FleetMissionStatus.Preparing => ["launch", "cancel"],
                FleetMissionStatus.Outbound => ["process", "recall"],
                FleetMissionStatus.Processing => ["return", "complete"],
                FleetMissionStatus.Returning => ["return", "complete"],
                FleetMissionStatus.Cancelled => ["cancel"],
                _ => ["complete"]
            };
            foreach (var operation in new[] { "launch", "process", "return", "recall", "complete", "cancel" }.Except(allowed))
                yield return new object[] { status, operation };
        }
    }

    [Theory]
    [MemberData(nameof(IllegalTransitions))]
    public void OutOfOrderAndTerminalMutationsAreRejected(FleetMissionStatus status, string operation)
    {
        var mission = InState(status);
        Action action = operation switch
        {
            "launch" => mission.StartOutbound,
            "process" => () => mission.BeginProcessing(Arrival),
            "return" => () => mission.BeginReturn(Arrival, ReturnArrival),
            "recall" => () => mission.Recall(RecallAt, RecallArrival),
            "complete" => () => mission.Complete(mission.CompletedAtUtc ?? ReturnArrival),
            "cancel" => () => mission.Cancel(Created),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
        RejectWithoutMutation<InvalidOperationException>(mission, action);
    }

    [Fact]
    public async Task EfModelMapsVersionAndLookupIndexesAndRoundTripsScalarState()
    {
        await using var db = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var entity = db.Model.FindEntityType(typeof(FleetMission))!;
        Assert.Equal(nameof(FleetMission.Id), Assert.Single(entity.FindPrimaryKey()!.Properties).Name);
        var version = entity.FindProperty(nameof(FleetMission.StateVersion))!;
        Assert.True(version.IsConcurrencyToken);
        Assert.Equal(ValueGenerated.Never, version.ValueGenerated);
        var optional = new[] { "ReturnDepartureUtc", "ReturnArrivalUtc", "CompletedAtUtc", "RecalledAtUtc" };
        Assert.All(entity.GetProperties(), property => Assert.Equal(optional.Contains(property.Name), property.IsNullable));
        foreach (var columns in new[] { new[] { "CivilizationId" }, ["OriginPlanetId"], ["DestinationPlanetId"],
            ["Status", "OutboundArrivalUtc"], ["Status", "ReturnArrivalUtc"] })
            Assert.Contains(entity.GetIndexes(), index => index.Properties.Select(property => property.Name).SequenceEqual(columns));
        var missions = Enum.GetValues<FleetMissionStatus>().Select(InState).ToArray();
        db.FleetMissions.AddRange(missions);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var persisted = await db.FleetMissions.ToDictionaryAsync(mission => mission.Id);
        Assert.Equal(missions.Length, persisted.Count);
        foreach (var mission in missions) Assert.Equal(Snapshot(mission), Snapshot(persisted[mission.Id]));
        // This checks EF metadata/materialization only, not relational race safety.
    }

    private static FleetMission Create(FleetMissionType type = FleetMissionType.Deploy) =>
        FleetMission.CreatePreparing(Civilization, Origin, Destination, type, Created, Departure, Arrival);

    private static FleetMission InState(FleetMissionStatus status)
    {
        var mission = Create();
        if (status == FleetMissionStatus.Preparing) return mission;
        if (status == FleetMissionStatus.Cancelled) { mission.Cancel(Created); return mission; }
        mission.StartOutbound();
        if (status == FleetMissionStatus.Outbound) return mission;
        if (status == FleetMissionStatus.Recalled)
        {
            mission.Recall(RecallAt, RecallArrival);
            mission.Complete(RecallArrival);
            return mission;
        }
        mission.BeginProcessing(Arrival);
        if (status == FleetMissionStatus.Processing) return mission;
        mission.BeginReturn(Arrival, ReturnArrival);
        if (status == FleetMissionStatus.Completed) mission.Complete(ReturnArrival);
        return mission;
    }

    private static object?[] Snapshot(FleetMission mission) =>
        typeof(FleetMission).GetProperties().OrderBy(property => property.Name).Select(property => property.GetValue(mission)).ToArray();

    private static void RejectWithoutMutation<TException>(FleetMission mission, Action action) where TException : Exception
    {
        var before = Snapshot(mission);
        Assert.Throws<TException>(action);
        Assert.Equal(before, Snapshot(mission));
    }
}

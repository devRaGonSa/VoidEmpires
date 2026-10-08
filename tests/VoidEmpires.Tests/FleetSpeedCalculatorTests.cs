using System.Globalization;
using System.Reflection;
using System.Text.Json;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class FleetSpeedCalculatorTests
{
    // Bind once: synthetic profiles exercise unreachable v1 catalog faults/ties without changing global metadata.
    private static readonly Func<IEnumerable<FleetSpeedShipInput>, decimal, Func<SpaceAssetType, decimal>, FleetSpeedResult> CalculateCore =
        typeof(FleetSpeedCalculator).GetMethod("CalculateCore", BindingFlags.NonPublic | BindingFlags.Static)!
            .CreateDelegate<Func<IEnumerable<FleetSpeedShipInput>, decimal, Func<SpaceAssetType, decimal>, FleetSpeedResult>>();

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft, 120)]
    [InlineData(SpaceAssetType.CargoCraft, 80)]
    [InlineData(SpaceAssetType.EscortCraft, 100)]
    [InlineData(SpaceAssetType.ColonyCraft, 60)]
    public void SingleTypeUsesCatalogSpeedAndIdentityModifier(SpaceAssetType type, int expectedSpeed)
    {
        var result = FleetSpeedCalculator.Calculate([new(type, 1)]);
        Assert.Equal(new FleetSpeedResult(expectedSpeed, 1m, expectedSpeed, type), result);
        Assert.Equal(OrbitalAssetCatalog.Get(type).BaseMovementSpeed, result.BaseSpeed);
        Assert.Equal(result, FleetSpeedCalculator.Calculate([new(type, 1)], 1.0m));
    }

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft, SpaceAssetType.EscortCraft, 100, SpaceAssetType.EscortCraft)]
    [InlineData(SpaceAssetType.ScoutCraft, SpaceAssetType.CargoCraft, 80, SpaceAssetType.CargoCraft)]
    [InlineData(SpaceAssetType.EscortCraft, SpaceAssetType.ColonyCraft, 60, SpaceAssetType.ColonyCraft)]
    public void MixedFleetUsesSlowestTypeRatherThanFastestSumOrAverage(SpaceAssetType first, SpaceAssetType second,
        int expectedSpeed, SpaceAssetType limiter)
    {
        Assert.Equal(new FleetSpeedResult(expectedSpeed, 1m, expectedSpeed, limiter),
            FleetSpeedCalculator.Calculate([new(first, 10), new(second, 2)]));
    }

    [Fact]
    public void ThreeTypeExampleIsLimitedByCargoAndAddingColonyLowersTheSpeed()
    {
        FleetSpeedShipInput[] ships = [new(SpaceAssetType.ScoutCraft, 10), new(SpaceAssetType.EscortCraft, 5), new(SpaceAssetType.CargoCraft, 2)];
        Assert.Equal(new FleetSpeedResult(80m, 1m, 80m, SpaceAssetType.CargoCraft), FleetSpeedCalculator.Calculate(ships));
        Assert.Equal(new FleetSpeedResult(60m, 1m, 60m, SpaceAssetType.ColonyCraft),
            FleetSpeedCalculator.Calculate([.. ships, new(SpaceAssetType.ColonyCraft, 1)]));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100000)]
    [InlineData(int.MaxValue)]
    public void QuantityDoesNotScaleOrWeightSingleOrMixedSpeed(int quantity)
    {
        Assert.Equal(FleetSpeedCalculator.Calculate([new(SpaceAssetType.CargoCraft, 1)]),
            FleetSpeedCalculator.Calculate([new(SpaceAssetType.CargoCraft, quantity)]));
        var expected = FleetSpeedCalculator.Calculate([new(SpaceAssetType.ScoutCraft, 1), new(SpaceAssetType.CargoCraft, 1)]);
        Assert.Equal(expected, FleetSpeedCalculator.Calculate([new(SpaceAssetType.ScoutCraft, quantity), new(SpaceAssetType.CargoCraft, 200)]));
    }

    [Fact]
    public void AllPermutationsAndRepeatedCallsProduceTheSameValue()
    {
        var inputs = AllTypes();
        var expected = new FleetSpeedResult(60m, 1.25m, 75m, SpaceAssetType.ColonyCraft);
        var permutations = Permutations(inputs).ToArray();
        Assert.Equal(24, permutations.Length);
        foreach (var permutation in permutations)
        {
            Assert.Equal(expected, FleetSpeedCalculator.Calculate(permutation, 1.25m));
            Assert.Equal(expected, FleetSpeedCalculator.Calculate(permutation, 1.25m));
        }
    }

    [Fact]
    public void DuplicateTypesMatchNormalizedInputWithoutSummingQuantities()
    {
        var normalized = FleetSpeedCalculator.Calculate([new(SpaceAssetType.ScoutCraft, 6), new(SpaceAssetType.CargoCraft, 2)]);
        var duplicates = FleetSpeedCalculator.Calculate([new(SpaceAssetType.ScoutCraft, 1), new(SpaceAssetType.ScoutCraft, 5), new(SpaceAssetType.CargoCraft, 2)]);
        Assert.Equal(normalized, duplicates);
        Assert.Equal(normalized, FleetSpeedCalculator.Calculate([
            new(SpaceAssetType.CargoCraft, int.MaxValue), new(SpaceAssetType.CargoCraft, int.MaxValue),
            new(SpaceAssetType.ScoutCraft, int.MaxValue), new(SpaceAssetType.ScoutCraft, int.MaxValue)]));
    }

    [Fact]
    public void LazyCompositionIsEnumeratedExactlyOnce()
    {
        var enumerations = 0;
        IEnumerable<FleetSpeedShipInput> Once()
        {
            if (++enumerations > 1) throw new InvalidOperationException("Input cannot be enumerated twice.");
            yield return new(SpaceAssetType.ScoutCraft, 1);
            yield return new(SpaceAssetType.CargoCraft, 1);
        }
        Assert.Equal(new FleetSpeedResult(80m, 1m, 80m, SpaceAssetType.CargoCraft), FleetSpeedCalculator.Calculate(Once()));
        Assert.Equal(1, enumerations);
    }

    [Fact]
    public void NullEmptyAndNullRowInputsAreRejectedExplicitly()
    {
        Assert.Throws<ArgumentNullException>(() => FleetSpeedCalculator.Calculate(null!));
        Assert.Throws<ArgumentException>(() => FleetSpeedCalculator.Calculate([]));
        Assert.Throws<ArgumentException>(() => FleetSpeedCalculator.Calculate([new(SpaceAssetType.ColonyCraft, 1), null!]));
    }

    public static IEnumerable<object[]> InvalidQuantities =>
        from type in Enum.GetValues<SpaceAssetType>()
        from quantity in new[] { 0, -1, int.MinValue }
        select new object[] { type, quantity };

    [Theory]
    [MemberData(nameof(InvalidQuantities))]
    public void EveryRowIncludingLateDuplicatesMustHavePositiveQuantity(SpaceAssetType type, int quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetSpeedCalculator.Calculate([new(type, quantity)]));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetSpeedCalculator.Calculate([
            new(SpaceAssetType.ColonyCraft, 1), new(type, 1), new(type, quantity)]));
    }

    [Theory]
    [InlineData((SpaceAssetType)0)]
    [InlineData((SpaceAssetType)(-1))]
    [InlineData((SpaceAssetType)5)]
    [InlineData((SpaceAssetType)int.MaxValue)]
    public void UnknownTypesAreRejectedBeforeCatalogLookupEvenAfterTheSlowestShip(SpaceAssetType type)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetSpeedCalculator.Calculate([new(type, 1)]));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetSpeedCalculator.Calculate([new(SpaceAssetType.ColonyCraft, 1), new(type, 1)]));
    }

    public static IEnumerable<object[]> Modifiers =>
    [
        [1m, 80m], [1.10m, 88m], [1.25m, 100m], [0.50m, 40m], [1.2345m, 98.76m], [1e-28m, 8e-27m]
    ];

    [Theory]
    [MemberData(nameof(Modifiers))]
    public void TrustedPositiveModifiersApplyAfterBaseSelectionWithoutRoundingPolicy(decimal modifier, decimal expected)
    {
        var result = FleetSpeedCalculator.Calculate([new(SpaceAssetType.ScoutCraft, 1), new(SpaceAssetType.CargoCraft, 1)], modifier);
        Assert.Equal(new FleetSpeedResult(80m, modifier, expected, SpaceAssetType.CargoCraft), result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-0.0001")]
    [InlineData("-79228162514264337593543950335")]
    public void NonpositiveModifiersAreRejected(string value)
    {
        var modifier = decimal.Parse(value, CultureInfo.InvariantCulture);
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetSpeedCalculator.Calculate([new(SpaceAssetType.ScoutCraft, 1)], modifier));
    }

    [Fact]
    public void CheckedEffectiveSpeedOverflowDoesNotAffectSubsequentCalls()
    {
        FleetSpeedShipInput[] ships = [new(SpaceAssetType.CargoCraft, 1)];
        var expected = FleetSpeedCalculator.Calculate(ships);
        Assert.Throws<OverflowException>(() => FleetSpeedCalculator.Calculate(ships, decimal.MaxValue));
        Assert.Equal(expected, FleetSpeedCalculator.Calculate(ships));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FutureEqualSpeedsSelectTheLowestNumericLimitingTypeInEveryOrder(bool allTied)
    {
        decimal Resolve(SpaceAssetType type) => allTied || type is SpaceAssetType.CargoCraft or SpaceAssetType.EscortCraft
            ? 40m : OrbitalAssetCatalog.Get(type).BaseMovementSpeed;
        var expectedLimiter = allTied ? SpaceAssetType.ScoutCraft : SpaceAssetType.CargoCraft;
        foreach (var permutation in Permutations(AllTypes()))
            Assert.Equal(new FleetSpeedResult(40m, 1.25m, 50m, expectedLimiter), CalculateCore(permutation, 1.25m, Resolve));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void InvalidFutureMetadataFailsEvenForALateOtherwiseNonlimitingShip(int speed)
    {
        Assert.Throws<InvalidOperationException>(() => CalculateCore([
            new(SpaceAssetType.ColonyCraft, 1), new(SpaceAssetType.ScoutCraft, 1)], 1m,
            type => type == SpaceAssetType.ScoutCraft ? speed : OrbitalAssetCatalog.Get(type).BaseMovementSpeed));
    }

    [Fact]
    public void FuturePositiveFractionalMetadataCannotUnderflowToANonpositiveSuccess()
    {
        Assert.Throws<InvalidOperationException>(() => CalculateCore([new(SpaceAssetType.ScoutCraft, 1)], 1e-28m, _ => 1e-28m));
    }

    [Fact]
    public void PreviewAndMissionProjectionLeaveInputsCatalogAndAllMissionStateUntouched()
    {
        var now = new DateTime(2026, 10, 8, 12, 0, 0, DateTimeKind.Utc);
        var mission = FleetMission.CreatePreparing(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FleetMissionType.Transport,
            now, now, now.AddHours(1));
        mission.AddShips(SpaceAssetType.ScoutCraft, 10);
        mission.AddShips(SpaceAssetType.EscortCraft, 5);
        mission.AddShips(SpaceAssetType.CargoCraft, 2);
        mission.AddCargo(ResourceType.Metal, 20);
        var inputs = mission.Ships.Select(ship => new FleetSpeedShipInput(ship.AssetType, ship.Quantity)).ToArray();
        var beforeInputs = inputs.ToArray();
        var beforeMission = JsonSerializer.Serialize(mission);
        var beforeCatalog = CatalogSnapshot();
        var expected = new FleetSpeedResult(80m, 1.25m, 100m, SpaceAssetType.CargoCraft);
        Assert.Equal(expected, FleetSpeedCalculator.Calculate(inputs, 1.25m));
        Assert.Equal(expected, FleetSpeedCalculator.Calculate(mission.Ships.Select(ship => new FleetSpeedShipInput(ship.AssetType, ship.Quantity)), 1.25m));
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetSpeedCalculator.Calculate(inputs, 0));
        Assert.Throws<OverflowException>(() => FleetSpeedCalculator.Calculate(inputs, decimal.MaxValue));
        Assert.Equal(beforeInputs, inputs);
        Assert.Equal(beforeMission, JsonSerializer.Serialize(mission));
        Assert.Equal(beforeCatalog, CatalogSnapshot());
    }

    [Fact]
    public void EmptyMissionLifecycleStillAllowsPreparationAndLaunchWithoutASpeedSuccess()
    {
        var now = new DateTime(2026, 10, 8, 12, 0, 0, DateTimeKind.Utc);
        var mission = FleetMission.CreatePreparing(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), FleetMissionType.Transport,
            now, now, now.AddHours(1));
        Assert.Throws<ArgumentException>(() => FleetSpeedCalculator.Calculate(mission.Ships.Select(ship => new FleetSpeedShipInput(ship.AssetType, ship.Quantity))));
        Assert.Equal((FleetMissionStatus.Preparing, 0), (mission.Status, mission.StateVersion));
        mission.StartOutbound();
        Assert.Equal(FleetMissionStatus.Outbound, mission.Status);
    }

    private static FleetSpeedShipInput[] AllTypes() =>
        Enum.GetValues<SpaceAssetType>().Select(type => new FleetSpeedShipInput(type, 1)).ToArray();
    private static string CatalogSnapshot() => JsonSerializer.Serialize(Enum.GetValues<SpaceAssetType>().Select(OrbitalAssetCatalog.Get));
    private static IEnumerable<FleetSpeedShipInput[]> Permutations(FleetSpeedShipInput[] ships)
    {
        if (ships.Length == 0)
        {
            yield return [];
            yield break;
        }
        for (var i = 0; i < ships.Length; i++)
            foreach (var tail in Permutations(ships.Where((_, index) => index != i).ToArray()))
                yield return [ships[i], .. tail];
    }
}

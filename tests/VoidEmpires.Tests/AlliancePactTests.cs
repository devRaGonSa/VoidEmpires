using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Tests;

public class AlliancePactTests
{
    [Fact]
    public void CreateAcceptsValidAlliancePact()
    {
        var sourceAllianceId = Guid.NewGuid();
        var targetAllianceId = Guid.NewGuid();
        var createdAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        var pact = AlliancePact.Create(
            sourceAllianceId,
            targetAllianceId,
            AlliancePactType.Cooperation,
            AlliancePactStatus.Active,
            createdAtUtc);

        Assert.NotEqual(Guid.Empty, pact.Id);
        Assert.Equal(sourceAllianceId, pact.SourceAllianceId);
        Assert.Equal(targetAllianceId, pact.TargetAllianceId);
        Assert.Equal(AlliancePactType.Cooperation, pact.PactType);
        Assert.Equal(AlliancePactStatus.Active, pact.Status);
        Assert.Equal(createdAtUtc, pact.CreatedAtUtc);
    }

    [Fact]
    public void CreateRejectsSelfPact()
    {
        var allianceId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => AlliancePact.Create(
            allianceId,
            allianceId,
            AlliancePactType.NonAggression,
            AlliancePactStatus.Proposed,
            DateTime.UtcNow));
    }

    [Fact]
    public void CreateRejectsInvalidInput()
    {
        Assert.Throws<ArgumentException>(() => AlliancePact.Create(
            Guid.Empty,
            Guid.NewGuid(),
            AlliancePactType.NonAggression,
            AlliancePactStatus.Proposed,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AlliancePact.Create(
            Guid.NewGuid(),
            Guid.Empty,
            AlliancePactType.NonAggression,
            AlliancePactStatus.Proposed,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AlliancePact.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            AlliancePactType.Unknown,
            AlliancePactStatus.Proposed,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AlliancePact.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            AlliancePactType.NonAggression,
            AlliancePactStatus.Unknown,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AlliancePact.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            AlliancePactType.NonAggression,
            AlliancePactStatus.Active,
            DateTime.Now));
    }
}

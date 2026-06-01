using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Tests;

public class AllianceTests
{
    [Fact]
    public void CreateAcceptsValidAllianceAndMembership()
    {
        var createdAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var joinedAtUtc = createdAtUtc.AddMinutes(5);
        var alliance = Alliance.Create(" Void Council ", " VC ", AllianceStatus.Active, createdAtUtc);
        var membership = AllianceMembership.Create(
            alliance.Id,
            Guid.NewGuid(),
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Leader,
            joinedAtUtc);

        Assert.NotEqual(Guid.Empty, alliance.Id);
        Assert.Equal("Void Council", alliance.Name);
        Assert.Equal("VC", alliance.Tag);
        Assert.Equal(AllianceStatus.Active, alliance.Status);
        Assert.Equal(createdAtUtc, alliance.CreatedAtUtc);
        Assert.Equal(alliance.Id, membership.AllianceId);
        Assert.Equal(AllianceMembershipStatus.Active, membership.Status);
        Assert.Equal(AllianceMembershipRole.Leader, membership.Role);
        Assert.Equal(joinedAtUtc, membership.JoinedAtUtc);
    }

    [Fact]
    public void CreateRejectsInvalidAllianceInput()
    {
        Assert.Throws<ArgumentException>(() => Alliance.Create("", "VC", AllianceStatus.Active, DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => Alliance.Create("Void Council", "", AllianceStatus.Active, DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => Alliance.Create("Void Council", "VC", AllianceStatus.Unknown, DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => Alliance.Create("Void Council", "VC", AllianceStatus.Active, DateTime.Now));
    }

    [Fact]
    public void CreateRejectsInvalidMembershipInput()
    {
        Assert.Throws<ArgumentException>(() => AllianceMembership.Create(
            Guid.Empty,
            Guid.NewGuid(),
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Member,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AllianceMembership.Create(
            Guid.NewGuid(),
            Guid.Empty,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Member,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AllianceMembership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            AllianceMembershipStatus.Unknown,
            AllianceMembershipRole.Member,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AllianceMembership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Unknown,
            DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => AllianceMembership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Member,
            DateTime.Now));
    }
}

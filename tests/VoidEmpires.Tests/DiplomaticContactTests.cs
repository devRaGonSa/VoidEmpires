using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Tests;

public class DiplomaticContactTests
{
    [Fact]
    public void CreateAcceptsValidContact()
    {
        var civilizationId = Guid.NewGuid();
        var contactedCivilizationId = Guid.NewGuid();
        var discoveredAtUtc = DateTime.UtcNow;

        var contact = DiplomaticContact.Create(
            civilizationId,
            contactedCivilizationId,
            DiplomaticContactStatus.Contacted,
            discoveredAtUtc,
            "manual-dev");

        Assert.NotEqual(Guid.Empty, contact.Id);
        Assert.Equal(civilizationId, contact.CivilizationId);
        Assert.Equal(contactedCivilizationId, contact.ContactedCivilizationId);
        Assert.Equal(DiplomaticContactStatus.Contacted, contact.Status);
        Assert.Equal(discoveredAtUtc, contact.DiscoveredAtUtc);
        Assert.Equal("manual-dev", contact.Source);
    }

    [Fact]
    public void CreateRejectsSelfContact()
    {
        var civilizationId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => DiplomaticContact.Create(
            civilizationId,
            civilizationId,
            DiplomaticContactStatus.Contacted,
            DateTime.UtcNow,
            "manual-dev"));
    }

    [Fact]
    public void CreateRejectsNonUtcTimestamp()
    {
        Assert.Throws<ArgumentException>(() => DiplomaticContact.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DiplomaticContactStatus.Contacted,
            DateTime.Now,
            "manual-dev"));
    }
}

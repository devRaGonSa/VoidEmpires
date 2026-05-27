using VoidEmpires.Domain.Players;

namespace VoidEmpires.Tests;

public class PlayerCivilizationDomainTests
{
    [Fact]
    public void PlayerProfileRejectsEmptyUserId()
    {
        Assert.Throws<ArgumentException>(() => PlayerProfile.Create(" ", "Player One"));
    }

    [Fact]
    public void PlayerProfileRejectsEmptyDisplayName()
    {
        Assert.Throws<ArgumentException>(() => PlayerProfile.Create("user-1", " "));
    }

    [Fact]
    public void CivilizationRejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(() =>
            Civilization.Create(Guid.NewGuid(), " ", CivilizationArchetype.Balanced));
    }

    [Fact]
    public void CivilizationRejectsEmptyPlayerProfileId()
    {
        Assert.Throws<ArgumentException>(() =>
            Civilization.Create(Guid.Empty, "Solar Dominion", CivilizationArchetype.Balanced));
    }

    [Fact]
    public void PlayerProfileCanAddCivilization()
    {
        var profile = PlayerProfile.Create("user-1", "Player One");
        var civilization = Civilization.Create(profile.Id, "Solar Dominion", CivilizationArchetype.Industrial);

        profile.AddCivilization(civilization);

        Assert.Single(profile.Civilizations);
        Assert.Equal(profile.Id, civilization.PlayerProfileId);
    }

    [Fact]
    public void PlayerProfileRejectsDuplicateCivilizationName()
    {
        var profile = PlayerProfile.Create("user-1", "Player One");
        profile.AddCivilization(Civilization.Create(profile.Id, "Solar Dominion", CivilizationArchetype.Industrial));

        Assert.Throws<InvalidOperationException>(() =>
            profile.AddCivilization(Civilization.Create(profile.Id, "solar dominion", CivilizationArchetype.Scientific)));
    }

    [Fact]
    public void CivilizationCanStoreHomePlanetId()
    {
        var profileId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var civilization = Civilization.Create(profileId, "Solar Dominion", CivilizationArchetype.Exploratory, planetId);

        Assert.Equal(planetId, civilization.HomePlanetId);
        Assert.Equal(CivilizationStatus.Active, civilization.Status);
    }
}

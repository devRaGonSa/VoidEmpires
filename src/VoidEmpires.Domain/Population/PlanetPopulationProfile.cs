namespace VoidEmpires.Domain.Population;

public sealed class PlanetPopulationProfile
{
    private PlanetPopulationProfile() { }

    private PlanetPopulationProfile(
        Guid planetId,
        long totalPopulation,
        long baseRecruitablePopulation,
        long baseCrewCapacity)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.", nameof(planetId));
        }

        if (totalPopulation < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalPopulation));
        }

        if (baseRecruitablePopulation < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(baseRecruitablePopulation));
        }

        if (baseCrewCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(baseCrewCapacity));
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
        TotalPopulation = totalPopulation;
        BaseRecruitablePopulation = baseRecruitablePopulation;
        BaseCrewCapacity = baseCrewCapacity;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public long TotalPopulation { get; private set; }
    public long BaseRecruitablePopulation { get; private set; }
    public long BaseCrewCapacity { get; private set; }

    public static PlanetPopulationProfile Create(
        Guid planetId,
        long totalPopulation,
        long baseRecruitablePopulation,
        long baseCrewCapacity)
        => new(planetId, totalPopulation, baseRecruitablePopulation, baseCrewCapacity);

    public bool CanRecruit(long requiredPopulation, long buildingBonusCapacity = 0)
    {
        if (requiredPopulation < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requiredPopulation));
        }

        if (buildingBonusCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(buildingBonusCapacity));
        }

        return requiredPopulation <= BaseRecruitablePopulation + buildingBonusCapacity;
    }

    public bool CanCrewLocallyBuiltShips(long requiredCrew, long buildingBonusCapacity = 0)
    {
        if (requiredCrew < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requiredCrew));
        }

        if (buildingBonusCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(buildingBonusCapacity));
        }

        return requiredCrew <= BaseCrewCapacity + buildingBonusCapacity;
    }
}
